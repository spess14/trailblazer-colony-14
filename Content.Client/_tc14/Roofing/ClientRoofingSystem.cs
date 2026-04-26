using Content.Shared._tc14.Roofing;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._tc14.Roofing;

public sealed class ClientRoofingSystem : RoofingSystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    public override bool OnToggleRoofOverlay(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
    {
        if (_overlay.HasOverlay<RoofingOverlay>())
        {
            _overlay.RemoveOverlay<RoofingOverlay>();
            _popup.PopupClient(Loc.GetString("roofing-overlay-off"), session?.AttachedEntity, PopupType.Medium);
        }
        else
        {
            _overlay.AddOverlay(new RoofingOverlay());
            _popup.PopupClient(Loc.GetString("roofing-overlay-on"), session?.AttachedEntity, PopupType.Medium);
        }

        return true;
    }
}
