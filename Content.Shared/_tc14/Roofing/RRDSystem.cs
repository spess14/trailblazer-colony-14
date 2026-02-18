using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Shared._tc14.Roofing;

/// <summary>
/// Manages the RRD.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class RRDSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedRoofSystem _roof = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RRDComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<RRDComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<RRDComponent, ActivateInWorldEvent>(OnActivateInWorld);
    }

    private void OnActivateInWorld(EntityUid uid, RRDComponent component, ref ActivateInWorldEvent args)
    {
        if (args.Handled)
            return;

        component.IsPlacingRoof = !component.IsPlacingRoof;
        var message = component.IsPlacingRoof ? Loc.GetString("rrd-now-placing") : Loc.GetString("rrd-now-removing");
        _popup.PopupPredicted(message, uid, args.User);
        Dirty(uid, component);
    }

    // Shamelessly copied from SoilManagerSystem, and the SoilManagerSystem had this copied from somewhere else.
    // Surely there must be a helper function for this.
    private bool GetInteractedWithTileDef(ref AfterInteractEvent args, out TileRef tileRef)
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

    //TODO move to an extension of SharedRoofSystem in _tc14 namespace
    public int HasNearbyRoof(Entity<MapGridComponent, RoofComponent> grid, Vector2i index)
    {
        var roofsCount = 0;
        var roofs = GetNearbyRoof(index);
        // not using LINQ since I might potentially make something that fills the room with roof
        foreach (var roof in roofs)
        {
            if (_roof.IsRooved(grid, roof))
                roofsCount += 1;
        }
        return roofsCount;
    }

    public List<Vector2i> GetNearbyRoof(Vector2i index)
    {
        return new List<Vector2i>
            { index + Vector2i.Down, index + Vector2i.Left, index + Vector2i.Up, index + Vector2i.Right };
    }

    private void OnAfterInteract(EntityUid uid, RRDComponent component, ref AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach)
            return;

        if (!GetInteractedWithTileDef(ref args, out var tileRef))
            return;

        var user = args.User;
        var location = args.ClickLocation;

        if (!location.IsValid(EntityManager))
            return;

        var gridUid = _transform.GetGrid(location);

        if (gridUid is null)
            return;

        if (!TryComp<MapGridComponent>(gridUid, out var mapGrid) || !TryComp<RoofComponent>(gridUid, out var roofComp))
            return;

        var grid = (gridUid.Value, mapGrid, roofComp);

        if (HasNearbyRoof(grid, tileRef.GridIndices) < 1 && component.IsPlacingRoof)
        {
            _popup.PopupPredicted(Loc.GetString("rrd-no-nearby-roof"), uid, user);
            return;
        }

        if (!component.IsPlacingRoof && !_roof.IsRooved(grid, tileRef.GridIndices))
        {
            _popup.PopupPredicted(Loc.GetString("rrd-no-roof-to-remove"), uid, user);
            return;
        }
        //TODO implement bearing roofs, disallowing people to just make floating pieces of roofing
        _roof.SetRoof(grid, tileRef.GridIndices, component.IsPlacingRoof);
        _audio.PlayPredicted(component.SuccessSound, uid, args.User);
    }

    private void OnExamine(EntityUid uid, RRDComponent component, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var msg = Loc.GetString(component.IsPlacingRoof ? "rrd-examine-constructing" : "rrd-examine-deconstructing");
        args.PushMarkup(msg);
    }
}
