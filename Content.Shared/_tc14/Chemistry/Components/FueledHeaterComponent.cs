using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Chemistry.Components;

/// <summary>
/// Hybrid solution-item heater.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FueledHeaterComponent : Component
{
    [DataField]
    public float SolutionHeatPerSecond;

    [DataField]
    public float EntityHeatPerSecond;
}
