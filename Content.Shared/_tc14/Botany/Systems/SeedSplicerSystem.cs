using Content.Shared._tc14.Botany.Components;
using Content.Shared.Botany.Items.Components;
using Content.Shared.Containers.ItemSlots;

namespace Content.Shared._tc14.Botany.Systems;

public sealed partial class SeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _slot = default!;
    private const string Slot1 = "seed1";
    private const string Slot2 = "seed2";

    // TODO
    public void SpliceSeed(Entity<SeedSplicerComponent> ent)
    {
        if (!_slot.TryGetSlot(ent.Owner, Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, Slot2, out var slot2))
            return;
        if (!TryComp<SeedComponent>(slot1.ContainerSlot?.ContainedEntity, out var seed1) || !TryComp<SeedComponent>(slot2.ContainerSlot?.ContainedEntity, out var seed2))
            return;
        PredictedSpawnAtPosition("WheatSeeds", Transform(ent).Coordinates);
    }
}
