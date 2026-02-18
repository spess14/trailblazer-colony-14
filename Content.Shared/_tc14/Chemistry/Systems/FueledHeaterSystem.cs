using Content.Shared._tc14.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Placeable;

namespace Content.Shared._tc14.Chemistry.Systems;

/// <summary>
/// Handles <see cref="FueledHeaterComponent"/>.
/// </summary>
public sealed class FueledHeaterSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;

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
        foreach (var heatingEntity in placer.PlacedEntities)
        {
            if (!TryComp<SolutionContainerManagerComponent>(heatingEntity, out var container))
                continue;

            var solutionEnergy = heater.SolutionHeatPerSecond * frameTime;
            foreach (var (_, soln) in _solution.EnumerateSolutions((heatingEntity, container)))
            {
                _solution.AddThermalEnergy(soln, solutionEnergy);
            }
        }
        var entityEnergy = heater.EntityHeatPerSecond * frameTime;
        foreach (var ent in placer.PlacedEntities)
        {
            var ev = new HeatEntityEvent
            {
                EntityUid = ent,
                Energy = entityEnergy,
            };
            RaiseLocalEvent(ref ev);
        }
    }
}
