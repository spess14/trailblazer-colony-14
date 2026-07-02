using System.Linq;
using System.Numerics;
using Content.Server._tc14.StationEvents.Components;
using Content.Server.GameTicking;
using Content.Server.Pinpointer;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Events;
using Content.Shared.EntityTable;
using Content.Shared.GameTicking.Components;
using Content.Shared.Humanoid;
using Content.Shared.Maps;
using Content.Shared.Objectives.Systems;
using Content.Shared.Physics;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._tc14.StationEvents.Events;

/// <summary>
/// Used to spawn entities not too far away from players.
/// </summary>
public sealed partial class PlanetEntitySummonRule : StationEventSystem<PlanetEntitySummonRuleComponent>
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private NavMapSystem _navMap = default!;
    [Dependency] private IMapManager _map = default!;
    [Dependency] private SharedMapSystem _smap = default!;
    [Dependency] private GameTicker _gameTicker = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private EntityTableSystem _table = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private TargetSystem _target = default!;
    [Dependency] private TurfSystem _turfSystem = default!;

    protected override void Added(EntityUid uid,
        PlanetEntitySummonRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleAddedEvent args)
    {
        var mapId = _gameTicker.DefaultMap;
        var mapGrid = _map.GetAllGrids(mapId)
            .First(grid => HasComp<MapGridComponent>(grid.Owner)
                           && !HasComp<BecomesStationComponent>(grid.Owner));
        Log.Info($"Map Grid: {mapGrid.Owner}");
        var players = EntityQuery<HumanoidProfileComponent, TransformComponent>();
        var positions = players.Select(player => player.Item2.Coordinates).ToList();
        var randomPlayerPosition = _random.Pick(positions).Position;
        var radius = new Vector2(component.MinDistance, component.MaxDistance);
        var targetCoords = SelectRandomTileInRange(randomPlayerPosition, radius, component.MaxAttempts, mapGrid);
        if (targetCoords is null)
        {
            Log.Warning($"No location found for {args.RuleId}!");
            ForceEndSelf(uid, gameRule);
            return;
        }

        var spawns = _table.GetSpawns(component.Table);

        foreach (var spawn in spawns)
        {
            var spawned = Spawn(spawn.Id, targetCoords.Value);
            Log.Info($"{targetCoords.Value.ToString()}");
        }
        base.Added(uid, component, gameRule, args);
    }

    /// <summary>
    /// Partially copied from ScramOnTrigger
    /// </summary>
    private EntityCoordinates? SelectRandomTileInRange(Vector2 point, Vector2 radius, int tries, Entity<MapGridComponent> ent)
    {
        EntityCoordinates? target = null;
        var baseCoords = new EntityCoordinates(ent.Owner, point);
        Log.Info($"Center: {point.ToString()}");
        for (var i = 0; i < tries; i++)
        {
            // Unlike ScramOnTrigger, we are trying to find a location further away with subsequent tries, not closer.
            var distance = (radius.Y - radius.X) * Math.Sqrt(_random.NextFloat()) * ((float)i / tries) + radius.X;
            var tempTargetCoords = baseCoords.Offset(_random.NextAngle().ToVec() * (float)distance);
            Log.Info($"{tempTargetCoords.ToString()}");
            if (!_smap.TryGetTileRef(ent.Owner, ent.Comp, tempTargetCoords, out var tileRef)
                || _turfSystem.IsTileBlocked(tileRef, CollisionGroup.MobMask))
                continue;
            target = tempTargetCoords;
            break;
        }
        return target;
    }
}
