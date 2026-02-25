using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Chemistry.Components;

/// <summary>
/// Hybrid solution-item heater.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FueledHeaterComponent : Component
{
    /// <summary>
    /// How much heat solutions get per second.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float SolutionHeatPerSecond;

    /// <summary>
    /// How much heat entities get per second (important for stuff like cooking meat)
    /// </summary>
    [DataField, AutoNetworkedField]
    public float EntityHeatPerSecond;

    /// <summary>
    /// Maximum temperature the heater can heat up things to.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int MaxTemp = 350;
}
