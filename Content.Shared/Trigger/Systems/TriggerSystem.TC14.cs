using Content.Shared._tc14.Research.Components;
using Content.Shared._tc14.Trigger.Components.Conditions;
using Content.Shared._tc14.Trigger.Components.Effects;
using Content.Shared.Damage.Systems;
using Robust.Shared.Network;

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
        SubscribeLocalEvent<TriggerOnDamageComponent, DamageChangedEvent>(OnDamageTrigger);
    }

    private void OnDamageTrigger(Entity<TriggerOnDamageComponent> ent, ref DamageChangedEvent args)
    {
        // Destructible system is unpredicted, and I really have no time to come up with a better solution.
        if (!args.DamageIncreased || _net.IsClient)
            return;
        Trigger(ent.Owner, args.Origin, ent.Comp.KeyOut);
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
        Dirty(ent, pointSourceComp);
    }
}
