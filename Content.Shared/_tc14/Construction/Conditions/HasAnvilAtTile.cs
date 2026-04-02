using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Content.Shared.Examine;
using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using YamlDotNet.Core.Tokens;

namespace Content.Shared._tc14.Construction.Conditions;

[UsedImplicitly, DataDefinition]
public sealed partial class HasAnvilAtTile : IGraphCondition
{
    private static readonly ProtoId<TagPrototype> AnvilTag = "Anvil";

    public bool Condition(EntityUid uid, IEntityManager entityManager)
    {
        if (!entityManager.TryGetComponent(uid, out TransformComponent? transform))
            return false;
        var location = transform.Coordinates;
        var sysMan = entityManager.EntitySysManager;
        var tagSystem = sysMan.GetEntitySystem<TagSystem>();
        var lookupSys = sysMan.GetEntitySystem<EntityLookupSystem>();

        foreach (var entity in lookupSys.GetEntitiesIntersecting(location, LookupFlags.Static))
        {
            if (tagSystem.HasTag(entity, AnvilTag))
                return true;
        }
        return false;
    }

    public bool DoExamine(ExaminedEvent args)
    {
        if (Condition(args.Examined, IoCManager.Resolve<IEntityManager>()))
            return false;

        args.PushMarkup(Loc.GetString("construction-step-condition-anvil-in-tile"));
        return true;
    }

    public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
    {
        yield return new ConstructionGuideEntry
        {
            Localization = "construction-step-condition-anvil-in-tile",
        };
    }
}
