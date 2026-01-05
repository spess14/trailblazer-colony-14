using Content.Shared._tc14.Research.Components;
using Content.Shared._tc14.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

/// <summary>
/// TriggerSystem partial containing all TC14-specific triggers.
/// Trigger/OnTrigger components still have to be fork-namespaced!
/// </summary>
public sealed partial class TriggerSystem
{
    // ReSharper disable once InconsistentNaming
    private void InitializeTC14()
    {
        SubscribeLocalEvent<AddResearchPointsOnTriggerComponent, TriggerEvent>(HandleAddResearchPointsOnTrigger);
    }

    private void HandleAddResearchPointsOnTrigger(Entity<AddResearchPointsOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null ||
            !EntityManager.TryGetComponent<TCResearchPointSourceComponent>(target, out var pointSourceComp))
            return;

        var triggerComp = ent.Comp;
        foreach (var pair in triggerComp.Points)
        {
            if (pointSourceComp.StoredPoints.ContainsKey(pair.Key))
            {
                pointSourceComp.StoredPoints[pair.Key] += pair.Value;
            }
            else
            {
                pointSourceComp.StoredPoints.Add(pair.Key, pair.Value);
            }
        }
    }
}
