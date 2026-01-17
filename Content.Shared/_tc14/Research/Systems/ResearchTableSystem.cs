using Content.Shared._tc14.Research.Components;
using Content.Shared._tc14.Research.Prototypes;
using Content.Shared.Research.Components;
using Content.Shared.Research.Systems;
using Content.Shared.UserInterface;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._tc14.Research.Systems;

/// <summary>
/// Handles ResearchTableComponent, also handles game logic for the UI.
/// </summary>
public sealed class ResearchTableSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly BlueprintSystem _blueprintSystem = default!;
    [Dependency] private readonly INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ResearchTableComponent, ResearchTableTechResearchedMessage>(OnResearchMessage);
        SubscribeLocalEvent<ResearchTableComponent, BeforeActivatableUIOpenEvent>(OnBeforeUiOpened);
        SubscribeLocalEvent<ResearchTableComponent, ResearchTablePrintBlueprint>(OnPrintMessage);
    }

    private void OnPrintMessage(Entity<ResearchTableComponent> ent, ref ResearchTablePrintBlueprint args)
    {
        var comp = ent.Comp;
        var id = args.Id;
        if (!IsResearched(id, ent, comp) || _timing.CurTime < comp.NextPrintTime || !_protoMan.Resolve(id, out var proto))
            return;
        comp.NextPrintTime = _timing.CurTime + comp.PrintDelay;
        if (_net.IsClient)
            return;
        var blueprint = Spawn("TCBlueprint", Transform(ent).Coordinates);
        var blueprintComp = EnsureComp<BlueprintComponent>(blueprint);
        _blueprintSystem.SetBlueprintRecipes((blueprint, blueprintComp), proto.UnlockedRecipes);
    }

    private void OnBeforeUiOpened(Entity<ResearchTableComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateUi(ent); // send the initial state to the UI
    }

    private void OnResearchMessage(Entity<ResearchTableComponent> ent, ref ResearchTableTechResearchedMessage args)
    {
        var comp = ent.Comp;
        var id = args.Id;
        ResearchEntry(id, ent, comp);
        UpdateUi(ent);
    }

    public void UpdateUi(Entity<ResearchTableComponent> ent)
    {
        if (!_uiSystem.HasUi(ent, ResearchTableUiKey.Key))
            return;
        var state = new ResearchTableState(ent.Comp.StoredPoints, ent.Comp.ResearchedTechs);
        _uiSystem.SetUiState(ent.Owner, ResearchTableUiKey.Key, state);
    }

    public bool IsResearched(ProtoId<ResearchEntryPrototype> entry, EntityUid uid, ResearchTableComponent? comp = null)
    {
        return Resolve(uid, ref comp) && comp.ResearchedTechs.Contains(entry);
    }

    public bool IsResearchable(ProtoId<ResearchEntryPrototype> entry, EntityUid uid, ResearchTableComponent? comp = null)
    {
        if (!Resolve(uid, ref comp) || !_protoMan.Resolve(entry, out var proto))
            return false;
        if (IsResearched(entry, uid, comp))
            return true;
        foreach (var dependencyId in proto.Dependencies)
        {
            if (!IsResearched(dependencyId, uid, comp))
                return false;
        }
        return true;
    }

    public bool HasPoints(ProtoId<ResearchDisciplinePrototype> discipline, int points, EntityUid uid, ResearchTableComponent? comp = null)
    {
        if (!Resolve(uid, ref comp) || !comp.StoredPoints.TryGetValue(discipline, out var pointAmount))
            return false;
        return pointAmount >= points;
    }

    public void ResearchEntry(ProtoId<ResearchEntryPrototype> entry, EntityUid uid, ResearchTableComponent? comp = null)
    {
        if (!Resolve(uid, ref comp) ||
            !_protoMan.Resolve(entry, out var proto) ||
            IsResearched(entry, uid, comp) ||
            !IsResearchable(entry, uid, comp) ||
            !HasPoints(proto.Discipline, proto.Points, uid, comp)
            )
            return;
        comp.StoredPoints[proto.Discipline] -= proto.Points;
        comp.ResearchedTechs.Add(entry);
        UpdateUi((uid, comp));
    }
}
