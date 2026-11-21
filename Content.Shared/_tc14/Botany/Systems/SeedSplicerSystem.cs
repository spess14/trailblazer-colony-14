using Content.Shared._tc14.Botany.Components;
using Content.Shared.Botany.Components;
using Content.Shared.Botany.Items.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Botany.Systems;

public sealed partial class SeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _slot = default!;
    [Dependency] private SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;
    [Dependency] private IEntityManager _entityManager = default!;

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
        string? seedName1 = null, seedName2 = null;
        if (TryComp<SeedComponent>(entUid1, out var seedComp1))
            seedProto1 = seedComp1.PlantProtoId;
        if (TryComp<SeedComponent>(entUid2, out var seedComp2))
            seedProto2 = seedComp2.PlantProtoId;
        if (_protoMan.Resolve(seedProto1, out var plant1) && plant1.TryComp<PlantDataComponent>(out var plantData1, _entityManager.ComponentFactory))
            seedName1 = plantData1.Name;
        if (_protoMan.Resolve(seedProto2, out var plant2) && plant2.TryComp<PlantDataComponent>(out var plantData2, _entityManager.ComponentFactory))
            seedName2 = plantData2.Name;
        var state = new SeedSplicerBoundUserInterfaceState(seedProto1, seedProto2, seedName1, seedName2);
        _userInterface.SetUiState(ent.Owner, SeedSplicerUiKey.Key, state);
    }
}
