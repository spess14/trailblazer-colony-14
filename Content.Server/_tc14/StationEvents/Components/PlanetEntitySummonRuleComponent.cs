using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Server._tc14.StationEvents.Components;

/// <summary>
/// Used to spawn entities not too far away from players.
/// </summary>
[RegisterComponent]
public sealed partial class PlanetEntitySummonRuleComponent : Component
{
    /// <summary>
    /// Which entities could be spawned?
    /// </summary>
    [DataField]
    public EntityTableSelector Table = default!;

    /// <summary>
    /// Maximum amount of attempts of trying a location to spawn these entities.
    /// </summary>
    [DataField]
    public int MaxAttempts = 96;

    /// <summary>
    /// The maximum distance to the closest player.
    /// </summary>
    [DataField]
    public int MaxDistance = 24;

    /// <summary>
    /// The entities will not spawn closer than this distance.
    /// </summary>
    [DataField]
    public int MinDistance = 8;
}
