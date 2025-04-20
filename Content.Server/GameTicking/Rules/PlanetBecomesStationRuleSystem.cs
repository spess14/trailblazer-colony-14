using System.Linq;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking;
using Content.Shared.GameTicking.Components;
using Content.Shared.Parallax.Biomes;
using Robust.Server.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.GameTicking.Rules;

/// <summary>
///     Adds the planet grid to the station and then removes the drop pod(s) from the station.
/// </summary>
public sealed class PlanetBecomesStationRuleSystem : GameRuleSystem<PlanetBecomesStationRuleComponent>
{
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly IMapManager _mapMan = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly IConsoleHost _host = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void Started(EntityUid uid,
        PlanetBecomesStationRuleComponent component,
        GameRuleComponent gameRule,
        GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        var mapId = _gameTicker.DefaultMap;
        var mapGrids = _mapMan.GetAllGrids(mapId).Select(grid => grid.Owner);
        //a bit evil but it'll do for now
        foreach (var grid in mapGrids)
        {
            if (!HasComp<BiomeComponent>(grid))
            {
                _host.ExecuteCommand($"stations:get stations:rmgrid {grid}");
                continue;
            }
            _host.ExecuteCommand($"stations:get stations:addgrid {grid}");
        }
    }
}
