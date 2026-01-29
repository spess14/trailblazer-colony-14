using Content.Shared._tc14.Research.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._tc14.Research.Systems;

/// <summary>
/// Handles TCResearchPointSourceComponent.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class TCResearchPointSourceSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ResearchTableSystem _researchTableSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TCResearchPointSourceComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<TCResearchPointSourceComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(Entity<TCResearchPointSourceComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target is null ||
            !args.CanReach ||
            !_entMan.TryGetComponent<ResearchTableComponent>(args.Target, out var tableComp))
            return;
        var sourceComp = ent.Comp;
        var pointsDeposited = 0;
        foreach (var pair in sourceComp.StoredPoints)
        {
            sourceComp.StoredPoints.Remove(pair.Key);
            if (tableComp.StoredPoints.ContainsKey(pair.Key))
            {
                tableComp.StoredPoints[pair.Key] += pair.Value;
            }
            else
            {
                tableComp.StoredPoints.Add(pair.Key, pair.Value);
            }
            pointsDeposited += pair.Value;
        }
        Dirty(args.Target.Value, tableComp);
        _researchTableSystem.UpdateUi((args.Target.Value, tableComp));
        _popup.PopupClient(pointsDeposited == 0
                ? Loc.GetString("observation-kit-deposit-nothing")
                : Loc.GetString("observation-kit-deposit-collected", ("points", pointsDeposited)),
            args.User);
    }

    private void OnExamine(Entity<TCResearchPointSourceComponent> ent, ref ExaminedEvent args)
    {
        var comp = ent.Comp;
        if (comp.StoredPoints.Count == 0 || !comp.HasExamine)
            return;
        using (args.PushGroup(nameof(TCResearchPointSourceComponent)))
        {
            args.PushMarkup(Loc.GetString("research-points-source-title"));
            foreach (var pair in comp.StoredPoints)
            {
                var discipline = _protoMan.Index(pair.Key);
                args.PushMarkup(Loc.GetString("research-points-source-stored",
                    ("name", Loc.GetString(discipline.Name)),
                    ("points", pair.Value)));
            }
        }
    }
}
