using Content.Shared._tc14.Damage.Components;
using Content.Shared.Damage.Components;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public sealed partial class DamageOnHoldingSystem : EntitySystem
{
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private DamageableSystem _damageableSystem = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private InventorySystem _inventorySystem = default!; // TC14 - add smithing gloves

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DamageOnHoldingComponent, MapInitEvent>(OnMapInit);
    }

    public void SetEnabled(EntityUid uid, bool enabled, DamageOnHoldingComponent? component = null)
    {
        if (Resolve(uid, ref component))
        {
            component.Enabled = enabled;
            component.NextDamage = _timing.CurTime;
        }
    }

    private void OnMapInit(EntityUid uid, DamageOnHoldingComponent component, MapInitEvent args)
    {
        component.NextDamage = _timing.CurTime;
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<DamageOnHoldingComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (!component.Enabled || component.NextDamage > _timing.CurTime)
                continue;
            if (_container.TryGetContainingContainer((uid, null, null), out var container))
            {
                // TC14 - Begin - add smithing gloves
                if (!_inventorySystem.TryGetInventoryEntity<DamageOnHoldingProtectionComponent>(container.Owner, out _))
                    _damageableSystem.TryChangeDamage(container.Owner, component.Damage, origin: uid);
                // TC14 - End
            }
            component.NextDamage = _timing.CurTime + TimeSpan.FromSeconds(component.Interval);
        }
    }
}
