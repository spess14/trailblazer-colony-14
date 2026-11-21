using Content.Shared._tc14.Botany.Components;
using Content.Shared.Botany.Items.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

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

    private void UpdateState(Entity<SeedSplicerComponent> ent, EntityEventArgs args)
    {
        if (!_slot.TryGetSlot(ent.Owner, Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, Slot2, out var slot2))
            return;
        var entUid1 = slot1.ContainerSlot?.ContainedEntity;
        var entUid2 = slot2.ContainerSlot?.ContainedEntity;
        EntProtoId? seedProto1 = null, seedProto2 = null;
        if (TryComp<SeedComponent>(entUid1, out var seedComp1))
            seedProto1 = seedComp1.PlantProtoId;
        if (TryComp<SeedComponent>(entUid2, out var seedComp2))
            seedProto2 = seedComp2.PlantProtoId;
        var state = new SeedSplicerBoundUserInterfaceState(GetNetEntity(entUid1) ?? NetEntity.Invalid, GetNetEntity(entUid2) ?? NetEntity.Invalid, seedProto1, seedProto2);
        _userInterface.SetUiState(ent.Owner, SeedSplicerUiKey.Key, state);
    }
}
