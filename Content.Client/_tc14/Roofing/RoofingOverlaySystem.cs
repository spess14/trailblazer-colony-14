using Content.Shared._tc14.Roofing;
using Robust.Client.Graphics;

namespace Content.Client._tc14.Roofing;

public sealed class RoofingOverlaySystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    private RoofingOverlay _overlay = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ToggleRoofingOverlayEvent>(OnToggle);
        _overlay = new();
    }

    private void OnToggle(ToggleRoofingOverlayEvent ev)
    {
        if (_overlayMan.HasOverlay<RoofingOverlay>())
        {
            _overlayMan.RemoveOverlay<RoofingOverlay>();
        }
        else
        {
            _overlayMan.AddOverlay(_overlay);
        }
    }
}
