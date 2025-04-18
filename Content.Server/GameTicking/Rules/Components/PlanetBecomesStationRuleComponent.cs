namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
///     This gamerule adds the planet grid to the station and then removes the drop pod(s) from the station.
///     Needed in order to make events actually run on the planet.
/// </summary>
[RegisterComponent]
public sealed partial class PlanetBecomesStationRuleComponent : Component;
