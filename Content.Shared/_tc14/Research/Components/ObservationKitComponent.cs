using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Research.Components;

/// <summary>
/// This component is added to observation kits - tools that allow you to collect points from research prototypes.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ObservationKitComponent : Component
{
    /// <summary>
    /// Points gathered from a prototype will be multiplied by this (as well as the user's research skill multiplier)
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 PointsMultiplier = 1;
}
