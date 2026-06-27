using System.Numerics;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Map.Components;

namespace Content.Client._tc14.Roofing;

public sealed class RoofingOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    private EntityLookupSystem _lookup;
    private SharedMapSystem _maps;
    private SharedRoofSystem _roofs;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;

    public RoofingOverlay()
    {
        IoCManager.InjectDependencies(this);

        _lookup = _entManager.System<EntityLookupSystem>();
        _maps = _entManager.System<SharedMapSystem>();
        _roofs = _entManager.System<SharedRoofSystem>();
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        var mapUid = _maps.GetMapOrInvalid(args.MapId);

        return _entManager.HasComponent<RoofComponent>(mapUid);
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var viewBounds = args.WorldBounds;
        var viewAABB = args.WorldAABB;
        var mapId = args.MapId;
        var mapUid = _maps.GetMapOrInvalid(args.MapId);
        if (!_entManager.TryGetComponent(mapUid, out MapGridComponent? mapGrid))
            return;
        if (!_entManager.TryGetComponent(mapUid, out RoofComponent? roofComp))
            return;
        var tiles = _maps.GetTilesEnumerator(mapUid, mapGrid, viewBounds);
        var drawHandle = (DrawingHandleWorld)args.DrawingHandle;

        while (tiles.MoveNext(out var tileRef))
        {
            var local = _lookup.GetLocalBounds(tileRef, mapGrid.TileSize);
            if (_roofs.IsRooved((mapUid, mapGrid, roofComp), tileRef.GridIndices))
                drawHandle.DrawRect(local, Color.Green.WithAlpha(0.5f));
        }
    }
}
