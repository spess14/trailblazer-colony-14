using Content.Shared._tc14.Roofing;
using Robust.Client.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._tc14.Roofing;

public sealed class ClientRoofingSystem : RoofingSystem
{
    [Dependency] private readonly IOverlayManager _overlay = default!;
    public override bool OnToggleRoofOverlay(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
    {
        if (_overlay.HasOverlay<RoofingOverlay>())
        {
            _overlay.RemoveOverlay<RoofingOverlay>();
        }
        else
        {
            _overlay.AddOverlay(new RoofingOverlay());
        }

        return true;
    }
}
