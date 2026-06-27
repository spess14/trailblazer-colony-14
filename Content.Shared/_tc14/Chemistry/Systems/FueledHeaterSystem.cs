using Content.Shared._tc14.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Placeable;
using Content.Shared.Temperature.Components;
using Content.Shared.Temperature.Systems;

namespace Content.Shared._tc14.Chemistry.Systems;

/// <summary>
/// Handles <see cref="FueledHeaterComponent"/>.
/// </summary>
public sealed class FueledHeaterSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly SharedTemperatureSystem _temperature = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FueledHeaterComponent, ItemPlacerComponent>();
        while (query.MoveNext(out var uid, out var heater, out var placer))
        {
            UpdateHeater(uid, heater, placer,  frameTime);
        }
    }

    private void UpdateHeater(EntityUid uid, FueledHeaterComponent heater, ItemPlacerComponent placer, float frameTime)
    {
        var entityCount = placer.PlacedEntities.Count;
        foreach (var heatingEntity in placer.PlacedEntities)
        {
            if (!TryComp<SolutionContainerManagerComponent>(heatingEntity, out var container))
                continue;

            var solutionEnergy = heater.SolutionHeatPerSecond * frameTime / entityCount;
            foreach (var (_, soln) in _solution.EnumerateSolutions((heatingEntity, container)))
            {
                _solution.AddThermalEnergyClamped(soln, solutionEnergy, 0, heater.MaxTemp);
            }
        }
        var entityEnergy = heater.EntityHeatPerSecond * frameTime / entityCount;
        foreach (var ent in placer.PlacedEntities)
        {
            if (TryComp<TemperatureComponent>(ent, out var temperature) && temperature.CurrentTemperature < heater.MaxTemp)
                _temperature.ChangeHeat(ent, entityEnergy);
        }
    }
}
