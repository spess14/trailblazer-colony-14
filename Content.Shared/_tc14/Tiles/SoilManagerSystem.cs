using Content.Shared.Burial.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Tiles;

/// <summary>
///     Handles digging up soil.
/// </summary>
public sealed class SoilManagerSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedToolSystem _tool = default!;
    [Dependency] private readonly TileSystem _tile = default!;
    [Dependency] private readonly INetManager _net = default!;

    private static readonly ProtoId<ToolQualityPrototype> ChiselingQuality = "Chiseling";

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShovelComponent, AfterInteractEvent>(OnShovelAfterInteract);
        SubscribeLocalEvent<ToolComponent, AfterInteractEvent>(OnChiselAfterInteract);
    }

    public bool GetInteractedWithTileDef(ref AfterInteractEvent args, out TileRef tileRef)
    {
        tileRef = TileRef.Zero;
        if (!args.CanReach || args.Handled)
            return false;

        var location = args.ClickLocation.AlignWithClosestGridTile();
        var locationMap = location.ToMap(EntityManager, _transform);
        if (locationMap.MapId == MapId.Nullspace)
            return false;

        if (!TryComp<MapGridComponent>(location.EntityId, out var mapGrid))
            return false;
        var gridUid = location.EntityId;
        tileRef = _map.GetTileRef(gridUid, mapGrid, location);
        return true;
    }

    private void OnShovelAfterInteract(Entity<ShovelComponent> ent, ref AfterInteractEvent args)
    {
        if (!GetInteractedWithTileDef(ref args, out var tileRef))
            return;

        var tileDef = (ContentTileDefinition)_tileDefinitionManager[tileRef.Tile.TypeId];

        if (tileDef.SoilPrototypeName is null)
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager,
            args.User,
            2f / ent.Comp.SpeedModifier,
            new SoilDigEvent
            {
                SoilPrototypeName = tileDef.SoilPrototypeName.Value,
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

    private void OnChiselAfterInteract(Entity<ToolComponent> ent, ref AfterInteractEvent args)
    {
        if (!TryComp<ToolComponent>(ent, out var tool))
            return;
        if (!_tool.HasQuality(ent, ChiselingQuality, tool))
            return;
        if (!GetInteractedWithTileDef(ref args, out var tileRef))
            return;
        var tileDef = (ContentTileDefinition)_tileDefinitionManager[tileRef.Tile.TypeId];
        if (!tileDef.IsChiselable)
            return;
        if (!_tileDefinitionManager.TryGetDefinition("FloorPlanetStone", out var stoneDef))
            return;
        // do not do this on client to avoid ugly mispredicts
        if (!_net.IsClient)
        {
            _tile.ReplaceTile(tileRef, (ContentTileDefinition)stoneDef);
        }
    }
}
