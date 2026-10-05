using Content.Server.GameTicking; // TC14 - Rework solars
using Content.Server.Solar.EntitySystems; // TC14 - Rework solars
using Content.Shared;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.Map.Components; // TC14 - Rework solars
using Robust.Shared.Random;
using Robust.Shared.Timing; // TC14 - Rework solars

namespace Content.Server.Light.EntitySystems;

/// <inheritdoc/>
public sealed partial class LightCycleSystem : SharedLightCycleSystem
{
    [Dependency] private IRobustRandom _random = default!;
    // TC14 - Begin - Rework solars
    [Dependency] private PowerSolarSystem _solars = default!;
    [Dependency] private GameTicker _ticker = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private MetaDataSystem _metadata = default!;
    // TC14 - End

    // TC14 - Begin - Rework solars
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var mapQuery = AllEntityQuery<LightCycleComponent, MapLightComponent>();
        while (mapQuery.MoveNext(out var uid,  out var cycle, out var map))
        {
            if (!cycle.Running)
                continue;

            // We still iterate paused entities as we still want to override the lighting color and not have
            // it apply the server state
            var pausedTime = _metadata.GetPauseTime(uid);

            var time = (float) _timing.CurTime
                .Add(cycle.Offset)
                .Subtract(_ticker.RoundStartTimeSpan)
                .Subtract(pausedTime)
                .TotalSeconds;

            var lightLevel = CalculateLightLevel(cycle, time);
            _solars.GlobalCoverage = (float)Math.Clamp(lightLevel-1, 0, 1);
        }
    }
    // TC14 - End

    protected override void OnCycleMapInit(Entity<LightCycleComponent> ent, ref MapInitEvent args)
    {
        base.OnCycleMapInit(ent, ref args);

        if (ent.Comp.InitialOffset)
        {
            SetOffset(ent, _random.Next(ent.Comp.Duration));
        }
    }
}
