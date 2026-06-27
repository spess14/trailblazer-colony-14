using Content.Shared._tc14.Skills.Components;
using Content.Shared._tc14.Skills.Systems;
using Content.Shared.Destructible;
using Content.Shared.Mining;
using Content.Shared.Mining.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Mining;

/// <summary>
/// This handles creating ores when the entity is destroyed.
/// </summary>
public sealed class MiningSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PlayerSkillsSystem _skills = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<OreVeinComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<OreVeinComponent, DestructionEventArgs>(OnDestruction);
    }

    private void OnDestruction(EntityUid uid, OreVeinComponent component, DestructionEventArgs args)
    {
        if (component.CurrentOre == null)
            return;

        var proto = _proto.Index<OrePrototype>(component.CurrentOre);

        if (proto.OreEntity == null)
            return;

        var coords = Transform(uid).Coordinates;
        var toSpawn = _random.Next(proto.MinOreYield, proto.MaxOreYield+1);
        // TC14 Begin: spawn more ore based on excavation skill
        var maxSkill = 0;
        var query = EntityQueryEnumerator<PlayerSkillsComponent, TransformComponent>();
        while (query.MoveNext(out var pUid, out _, out var transformComp))
        {
            // Since we don't get the reason who(or what) broke the ore, get players in a small range and use the biggest skill.
            if (!_transform.InRange(coords, transformComp.Coordinates, 3f))
                continue;
            maxSkill = Math.Max(maxSkill, _skills.GetSkillLevel("SkillExcavation", pUid));
        }
        toSpawn += _skills.CumulativeChanceRoll(maxSkill * 0.1f); // You get a maximum of two additional drops max.
        // TC14 End: spawn more ore based on excavation skill
        for (var i = 0; i < toSpawn; i++)
        {
            Spawn(proto.OreEntity, coords.Offset(_random.NextVector2(0.2f)));
        }
    }

    private void OnMapInit(EntityUid uid, OreVeinComponent component, MapInitEvent args)
    {
        if (component.CurrentOre != null || component.OreRarityPrototypeId == null || !_random.Prob(component.OreChance))
            return;

        component.CurrentOre = _proto.Index<WeightedRandomOrePrototype>(component.OreRarityPrototypeId).Pick(_random);
    }
}
