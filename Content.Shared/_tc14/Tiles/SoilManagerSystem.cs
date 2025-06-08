using Content.Shared.Burial.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Shared._tc14.Tiles;

/// <summary>
///     Handles digging up soil.
/// </summary>
public sealed class SoilManagerSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShovelComponent, AfterInteractEvent>(OnAfterInteract);
    }

    public bool GetInteractedWithTileDef(ref AfterInteractEvent args, out ContentTileDefinition? tileDef)
    {
        tileDef = null;
        if (!args.CanReach || args.Handled)
            return false;

        var location = args.ClickLocation.AlignWithClosestGridTile();
        var locationMap = location.ToMap(EntityManager, _transform);
        if (locationMap.MapId == MapId.Nullspace)
            return false;

        var physicQuery = GetEntityQuery<PhysicsComponent>();
        var transformQuery = GetEntityQuery<TransformComponent>();

        var map = location.ToMap(EntityManager, _transform);

        var userPos = transformQuery.GetComponent(args.User).Coordinates.ToMapPos(EntityManager, _transform);
        var dir = userPos - map.Position;
        if (dir.LengthSquared() > 0.01)
        {
            var ray = new CollisionRay(map.Position, dir.Normalized(), (int) CollisionGroup.Impassable);
            var results = _physics.IntersectRay(locationMap.MapId, ray, dir.Length(), returnOnFirstHit: true);
        }

        if (!TryComp<MapGridComponent>(location.EntityId, out var mapGrid))
            return false;
        var gridUid = location.EntityId;
        var tile = _map.GetTileRef(gridUid, mapGrid, location);
        tileDef = (ContentTileDefinition) _tileDefinitionManager[tile.Tile.TypeId];
        return true;
    }

    private void OnAfterInteract(Entity<ShovelComponent> ent, ref AfterInteractEvent args)
    {
        if (!GetInteractedWithTileDef(ref args, out var tileDef))
            return;

        if (tileDef!.SoilPrototypeName is null)
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager,
            args.User,
            2f / ent.Comp.SpeedModifier,
            new SoilDigEvent
            {
                SoilPrototypeName = tileDef.SoilPrototypeName,
            },
            null,
            null,
            args.Used)
        {
            Broadcast = true,
            BreakOnMove = true,
            NeedHand = true,
        };
        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }
}
