using Content.Shared._tc14.Research.Components;
using Content.Shared._tc14.Research.Prototypes;
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
        var state = new ResearchTableState(ent.Comp.StoredPoints, ent.Comp.ResearchedTechs);
        _uiSystem.SetUiState(ent.Owner, ResearchTableUiKey.Key, state);
    }

    private void OnInit(Entity<ResearchTableComponent> ent, ref ComponentInit args)
    {
        var disciplines = _protoMan.EnumeratePrototypes<ResearchDisciplinePrototype>();
        foreach (var discipline in disciplines)
        {
            ent.Comp.StoredPoints.TryAdd(discipline.ID, 0);
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

    // TODO validate and subtract points
    public void ResearchEntry(ProtoId<ResearchEntryPrototype> entry, EntityUid uid, ResearchTableComponent? comp = null)
    {
        if (!Resolve(uid, ref comp) || IsResearched(entry, uid, comp) || !IsResearchable(entry, uid, comp))
            return;
        comp.ResearchedTechs.Add(entry);
    }
}
