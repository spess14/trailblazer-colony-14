using Content.Shared._tc14.Research.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Network;

namespace Content.Shared._tc14.Research.Systems;

/// <summary>
/// Handles ObservationKitComponent and interaction with TCResearchPointSourceComponent.
/// You need to add TCResearchPointSourceComponent for this to work.
/// </summary>
public sealed class ObservationKitSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ObservationKitComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(Entity<ObservationKitComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target is null ||
            !args.CanReach ||
            !_entMan.TryGetComponent<TCResearchPointSourceComponent>(args.Target, out var targetPointsComp) ||
            !_entMan.TryGetComponent<TCResearchPointSourceComponent>(ent, out var kitPointsComp) ||
            !targetPointsComp.CanBeObserved)
            return;
        var pointsCollected = 0;
        foreach (var pair in targetPointsComp.StoredPoints)
        {
            targetPointsComp.StoredPoints.Remove(pair.Key);
            if (kitPointsComp.StoredPoints.ContainsKey(pair.Key))
            {
                kitPointsComp.StoredPoints[pair.Key] += pair.Value;
            }
            else
            {
                kitPointsComp.StoredPoints.Add(pair.Key, pair.Value);
            }
            pointsCollected += pair.Value;
        }
        _popup.PopupClient(pointsCollected == 0
                ? Loc.GetString("observation-kit-gather-nothing")
                : Loc.GetString("observation-kit-gather-collected", ("points", pointsCollected)),
            args.User);
        Dirty(ent, kitPointsComp);
        if (!targetPointsComp.DestructOnObservation || pointsCollected <= 0)
            return;
        PredictedQueueDel(args.Target);
        args.Handled = true;
    }
}
