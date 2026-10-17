using System.Linq;
using System.Numerics;
using Content.Server._tc14.StationEvents.Components;
using Content.Server.GameTicking;
using Content.Server.StationEvents.Events;
using Content.Shared.EntityTable;
using Content.Shared.GameTicking.Components;
using Content.Shared.Humanoid;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Station.Components;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._tc14.StationEvents.Events;

/// <summary>
/// Used to spawn entities not too far away from players.
/// </summary>
public sealed partial class PlanetEntitySummonRule : StationEventSystem<PlanetEntitySummonRuleComponent>
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private ServerGameTicker _gameTicker = default!;
    [Dependency] private EntityTableSystem _table = default!;
    [Dependency] private TurfSystem _turfSystem = default!;

    protected override void Started(Entity<PlanetEntitySummonRuleComponent, GameRuleComponent> ent, ref GameRuleStartedEvent args)
    {
        base.Started(ent, ref args);
        var mapId = _gameTicker.DefaultMap;
        var mapGrid = _map.GetAllGrids(mapId)
            .FirstOrNull(grid => HasComp<MapGridComponent>(grid.Owner)
                           && !HasComp<BecomesStationComponent>(grid.Owner));
        if (mapGrid is null)
        {
            Log.Warning($"No valid grid found for {ToPrettyString(ent.Owner):rule}!");
            //ForceEndSelf((ent.Owner, ent.Comp2));
            return;
        }
        var players = EntityQuery<HumanoidProfileComponent, TransformComponent>();
        var positions = players.Select(player => player.Item2.Coordinates).ToList();
        if (positions.Count == 0)
        {
            Log.Warning($"No players found for {ToPrettyString(ent.Owner):rule}!");
            //ForceEndSelf((ent.Owner, ent.Comp2));
            return;
        }
        var randomPlayerPosition = _random.Pick(positions).Position;
        var radius = new Vector2(ent.Comp1.MinDistance, ent.Comp1.MaxDistance);
        var targetCoords = SelectRandomTileInRange(randomPlayerPosition, radius, ent.Comp1.MaxAttempts, mapGrid.Value);
        if (targetCoords is null)
        {
            Log.Warning($"No location found for {ToPrettyString(ent.Owner):rule}!");
            //ForceEndSelf((ent.Owner, ent.Comp2));
            return;
        }

        var spawns = _table.GetSpawns(ent.Comp1.Table);

        foreach (var spawn in spawns)
        {
            Spawn(spawn.Id, targetCoords.Value);
        }
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
            if (!_map.TryGetTileRef(ent.Owner, ent.Comp, tempTargetCoords, out var tileRef)
                || _turfSystem.IsTileBlocked(tileRef, CollisionGroup.MobMask))
                continue;
            target = tempTargetCoords;
            break;
        }
        return target;
    }
}
