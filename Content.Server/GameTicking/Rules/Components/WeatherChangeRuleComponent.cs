using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Random;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Changes the weather.
/// </summary>
[RegisterComponent]
public sealed partial class WeatherChangeRuleComponent : Component
{
    /// <summary>
    /// Weather selector prototype ID.
    /// </summary>
    [DataField]
    public ProtoId<WeightedRandomPrototype> WeatherTable;

    [DataField]
    public TimeSpan MinTime = TimeSpan.FromMinutes(1);

    [DataField]
    public TimeSpan MaxTime = TimeSpan.FromMinutes(10);
}
