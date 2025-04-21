using System.Linq;
using Content.Server.GameTicking.Rules.Components;
using Content.Shared.EntityTable;
using Content.Shared.GameTicking.Components;
using Content.Shared.Random.Helpers;
using Content.Shared.Weather;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Handles weather changes via event.
/// </summary>
public sealed class WeatherChangeRuleSystem : GameRuleSystem<WeatherChangeRuleComponent>
{
    [Dependency] private readonly EntityTableSystem _entityTable = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedWeatherSystem _weather = default!;
    protected override void Started(EntityUid uid,
        WeatherChangeRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        var map = GameTicker.DefaultMap;
        var newWeather = _protoManager.Index<WeatherPrototype>(_entityTable.GetSpawns(component.WeatherTable).First());
        _weather.SetWeather(map, newWeather, TimeSpan.FromMinutes(_random.NextFloat(component.MinTime.Minutes, component.MaxTime.Minutes)));
        GameTicker.EndGameRule(uid, gameRule);
    }
}
