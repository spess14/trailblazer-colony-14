using Content.Shared._tc14.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;

namespace Content.Shared._tc14.Chemistry.Systems;

public sealed partial class FermenterSystem : EntitySystem
{
    [SubscribeLocalEvent]
    private void OnSolutionChange(Entity<FermenterComponent> ent, ref SolutionChangedEvent args)
    {
        ent.Comp.NextInverval = TimeSpan.Zero;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FermenterComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            comp.NextInverval += TimeSpan.FromSeconds(frameTime);
            if (comp.NextInverval < comp.FermentingInterval)
                return;
            comp.NextInverval -= comp.FermentingInterval;
            var ev = new FermentEvent();
            RaiseLocalEvent(uid, ref ev);
        }
    }
}

[ByRefEvent]
public record struct FermentEvent();
