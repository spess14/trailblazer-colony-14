using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Chemistry.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FermenterComponent : Component
{
    /// <summary>
    /// By what modifier is the fermentation sped up?
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 SpeedModifier = FixedPoint2.Epsilon;

    /// <summary>
    /// Fermenter will check for valid reactions once per this interval.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan FermentingInterval = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Time passed since last interval.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan NextInverval = TimeSpan.Zero;
}
