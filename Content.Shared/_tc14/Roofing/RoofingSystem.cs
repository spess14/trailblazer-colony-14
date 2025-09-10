using Content.Shared._tc14.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Shared._tc14.Roofing;

/// <summary>
/// This handles...
/// </summary>
public abstract class RoofingSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        CommandBinds.Builder
            .Bind(ForkKeyFunctions.ToggleRoofOverlay, new PointerInputCmdHandler(OnToggleRoofOverlay))
            .Register<RoofingSystem>();
    }

    public virtual bool OnToggleRoofOverlay(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
    {
        return true;
    }
}
