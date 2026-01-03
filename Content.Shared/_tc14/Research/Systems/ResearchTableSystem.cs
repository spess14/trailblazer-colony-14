using System.Linq;
using Content.Shared._tc14.Research.Components;
using Content.Shared._tc14.Research.Prototypes;
using Content.Shared.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Research.Systems;

/// <summary>
/// Handles ResearchTableComponent, also handles game logic for the UI.
/// </summary>
public sealed class ResearchTableSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ResearchTableComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<ResearchTableComponent, ResearchTableTechResearchedMessage>(OnResearchMessage);
        SubscribeLocalEvent<ResearchTableComponent, BeforeActivatableUIOpenEvent>(OnBeforeUiOpened);
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

    private void UpdateUi(Entity<ResearchTableComponent> ent)
    {
        if (!_uiSystem.HasUi(ent, ResearchTableUiKey.Key))
            return;
        var state = new ResearchTableState(ent.Comp.StoredPoints, ent.Comp.ResearchedTechs);
        _uiSystem.SetUiState(ent.Owner, ResearchTableUiKey.Key, state);
    }

    private void OnInit(Entity<ResearchTableComponent> ent, ref ComponentInit args)
    {
        var disciplines = _protoMan.EnumeratePrototypes<ResearchDisciplinePrototype>();
        disciplines = disciplines.OrderByDescending(d => d.Priority);
        foreach (var discipline in disciplines)
        {
            ent.Comp.StoredPoints.TryAdd(discipline.ID, 1000);
        }
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

    // TODO validate and subtract points
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
