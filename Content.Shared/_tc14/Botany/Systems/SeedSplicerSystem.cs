using System.Linq;
using Content.Shared._tc14.Botany.Components;
using Content.Shared._tc14.Botany.Prototypes;
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

    // TODO
    public void SpliceSeed(Entity<SeedSplicerComponent> ent)
    {
        if (!_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot2, out var slot2))
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

    public SeedSplicePrototype? FindSplice(EntProtoId plantProto1, EntProtoId plantProto2)
    {
        var prototypes = _protoMan.EnumeratePrototypes<SeedSplicePrototype>();
        return prototypes.FirstOrDefault(p => p.Seed1 == plantProto1 && p.Seed2 == plantProto2);
    }

    public string? FindSpliceName(EntProtoId plantProto1, EntProtoId plantProto2)
    {
        return FindSpliceName(FindSplice(plantProto1, plantProto2));
    }

    public string? FindSpliceName(SeedSplicePrototype? proto)
    {
        if (proto is null)
            return null;
        if (_protoMan.Resolve(proto.OutputSeed, out var seed) &&
            seed.TryComp<SeedComponent>(out var seedComp, _entityManager.ComponentFactory) &&
            _protoMan.Resolve(seedComp.PlantProtoId, out var plant) &&
            plant.TryComp<PlantDataComponent>(out var plantData, _entityManager.ComponentFactory))
            return plantData.Name;
        return null;
    }

    private void UpdateState(Entity<SeedSplicerComponent> ent, EntityEventArgs args)
    {
        if (!_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot2, out var slot2))
            return;
        var entUid1 = slot1.ContainerSlot?.ContainedEntity;
        var entUid2 = slot2.ContainerSlot?.ContainedEntity;
        EntProtoId? plantProto1 = null, plantProto2 = null;
        EntProtoId? seedProto1 = null, seedProto2 = null;
        SeedSplicePrototype? resultProto = null;
        string? seedName1 = null, seedName2 = null, resultName = null;
        if (TryComp<SeedComponent>(entUid1, out var seedComp1))
            plantProto1 = seedComp1.PlantProtoId;
        if (TryComp<SeedComponent>(entUid2, out var seedComp2))
            plantProto2 = seedComp2.PlantProtoId;
        if (_protoMan.Resolve(plantProto1, out var plant1) &&
            plant1.TryComp<PlantDataComponent>(out var plantData1, _entityManager.ComponentFactory))
        {
            seedName1 = plantData1.Name;
            seedProto1 = plantData1.PacketPrototype;
        }
        if (_protoMan.Resolve(plantProto2, out var plant2) &&
            plant2.TryComp<PlantDataComponent>(out var plantData2, _entityManager.ComponentFactory))
        {
            seedName2 = plantData2.Name;
            seedProto2 = plantData2.PacketPrototype;
        }
        if (seedProto1 is not null && seedProto2 is not null)
        {
            resultProto = FindSplice(seedProto1.Value, seedProto2.Value);
            resultName = FindSpliceName(resultProto);
        }
        var state = new SeedSplicerBoundUserInterfaceState(seedName1, seedName2, resultName);
        _userInterface.SetUiState(ent.Owner, SeedSplicerUiKey.Key, state);
    }
}
