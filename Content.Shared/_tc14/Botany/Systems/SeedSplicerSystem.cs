using Content.Shared._tc14.Botany.Components;
using Content.Shared.Botany.Items.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;

namespace Content.Shared._tc14.Botany.Systems;

public sealed partial class SeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _slot = default!;
    [Dependency] private SharedUserInterfaceSystem _userInterface = default!;

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

    [SubscribeLocalEvent]
    private void OnInsert(Entity<SeedSplicerComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        UpdateState(ent, args);
    }

    [SubscribeLocalEvent]
    private void OnEject(Entity<SeedSplicerComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        UpdateState(ent, args);
    }

    // yeah, uh, this is a lot of hacky boilerplate
    private void UpdateState(Entity<SeedSplicerComponent> ent, EntityEventArgs args)
    {
        if (!_slot.TryGetSlot(ent.Owner, Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, Slot2, out var slot2))
            return;
        var entUid1 = slot1.ContainerSlot?.ContainedEntity;
        var entUid2 = slot2.ContainerSlot?.ContainedEntity;
        Entity<SeedComponent>? ent1 = null, ent2 = null;
        string? entName1 = null, entName2 = null;
        if (entUid1 is not null)
        {
            if (!TryComp<SeedComponent>(entUid1, out var seed1))
            {
                Log.Error($"Entity {entUid1} is not a seed, yet inserted into the seed splicer somehow.");
                return;
            }
            ent1 = (entUid1.Value, seed1);
            entName1 = Comp<MetaDataComponent>(entUid1.Value).EntityName;
        }
        if (entUid2 is not null)
        {
            if (!TryComp<SeedComponent>(entUid2, out var seed2))
            {
                Log.Error($"Entity {entUid2} is not a seed, yet inserted into the seed splicer somehow.");
                return;
            }
            ent2 = (entUid2.Value, seed2);
            entName2 = Comp<MetaDataComponent>(entUid2.Value).EntityName;
        }
        var state = new SeedSplicerBoundUserInterfaceState(ent1, ent2, entName1, entName2);
        _userInterface.SetUiState(ent.Owner, SeedSplicerUiKey.Key, state);
    }
}
