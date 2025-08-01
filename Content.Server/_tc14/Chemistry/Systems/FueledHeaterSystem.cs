using Content.Server.Temperature.Systems;
using Content.Shared._tc14.Chemistry;

namespace Content.Server._tc14.Chemistry.Systems;

public sealed class FueledHeaterSystem : EntitySystem
{
    [Dependency] private readonly TemperatureSystem _temperature = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HeatEntityEvent>(OnHeatEntity);
    }

    private void OnHeatEntity(ref HeatEntityEvent ev)
    {
        _temperature.ChangeHeat(ev.EntityUid, ev.Energy);
    }
}
