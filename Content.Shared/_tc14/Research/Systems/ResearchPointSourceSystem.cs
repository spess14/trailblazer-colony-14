using Content.Shared._tc14.Research.Components;
using Content.Shared.Examine;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Research.Systems;

/// <summary>
/// Handles ResearchPointSourceComponent.
/// </summary>
public sealed class ResearchPointSourceSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TCResearchPointSourceComponent, ExaminedEvent>(OnExamine);
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
