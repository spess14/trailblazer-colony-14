using Content.Shared.Hands.EntitySystems;
using Content.Shared.Light.EntitySystems;
using Content.Shared.Prying.Components;
using Content.Shared.Verbs;
using Robust.Shared.Map.Components;

namespace Content.Shared._tc14.Tiles;

/// <summary>
/// Handles various roof stuff, such as placing roofs down and whanot
/// </summary>
public sealed class RoofSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedRoofSystem _roofs = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PryingComponent, GetVerbsEvent<ActivationVerb>>(RemoveRoofVerb);
    }

    private void RemoveRoofVerb(Entity<PryingComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!_hands.IsHolding((args.User, args.Hands), ent.Owner, out var hand ))
            return;

        if (args.Hands?.ActiveHandId != hand)
            return;

        var @event = args;
        ActivationVerb verb = new()
        {
            Text = "bwahh just ujhh pry the roof",
            Act = () => TryPryRoof(ent, @event.User),
        };

        args.Verbs.Add(verb);
    }

    private bool TryPryRoof(Entity<PryingComponent> ent, EntityUid user)
    {
        var grid = _transform.GetGrid(user);

        if (grid == null)
            return false;

        if (!TryComp<MapGridComponent>((EntityUid)grid, out var map))
            return false;

        _roofs.SetRoof(((EntityUid)grid, map), _transform.GetGridOrMapTilePosition(user), false);

        return true;
    }
}
