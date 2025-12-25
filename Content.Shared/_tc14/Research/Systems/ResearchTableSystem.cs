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

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ResearchTableComponent, ComponentInit>(OnInit);
    }

    private void OnInit(Entity<ResearchTableComponent> ent, ref ComponentInit args)
    {
        var disciplines = _protoMan.EnumeratePrototypes<ResearchDisciplinePrototype>();
        foreach (var discipline in disciplines)
        {
            ent.Comp.StoredPoints.TryAdd(discipline.ID, 0);
        }
    }
}
