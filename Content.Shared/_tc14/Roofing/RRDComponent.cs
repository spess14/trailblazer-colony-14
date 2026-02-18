using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Roofing;

/// <summary>
/// Handles the rapid roofinf device.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(RRDSystem))]
// ReSharper disable once InconsistentNaming shut the fuck up
public sealed partial class RRDComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool IsPlacingRoof = true;

    [DataField, AutoNetworkedField]
    public SoundSpecifier SuccessSound = new SoundPathSpecifier("/Audio/Items/deconstruct.ogg");
}
