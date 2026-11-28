using System.Linq;
using Content.Shared._tc14.Botany.Components;
using Content.Shared._tc14.Botany.Prototypes;
using Content.Shared.Botany.Components;
using Content.Shared.Botany.Items.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Random.Helpers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._tc14.Botany.Systems;

public sealed partial class SeedSplicerSystem : EntitySystem
{
    [Dependency] private ItemSlotsSystem _slot = default!;
    [Dependency] private SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private IEntityManager _entityManager = default!;
    [Dependency] private INetManager _net = default!;

    [SubscribeLocalEvent]
    private void OnSplice(Entity<SeedSplicerComponent> ent, ref SeedSplicerSpliceEvent args)
    {
        if (!_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot2, out var slot2))
            return;
        var entUid1 = slot1.ContainerSlot?.ContainedEntity;
        var entUid2 = slot2.ContainerSlot?.ContainedEntity;
        GetSeedInfo(entUid1, out _, out var seedProto1, out _);
        GetSeedInfo(entUid2, out _, out var seedProto2, out _);
        if (seedProto1 is null || seedProto2 is null)
            return;
        var spliceProto = FindSplice(seedProto1.Value, seedProto2.Value);
        if (spliceProto is null)
            return;
        // TODO implement chance scaling with botany skill
        var random = SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent));
        QueueDel(entUid1);
        QueueDel(entUid2);
        SoundSpecifier sound;
        if (random.Next(100) < spliceProto.Chance * 100)
        {
            PredictedSpawnAtPosition(spliceProto.OutputSeed, Transform(ent).Coordinates);
            sound = ent.Comp.SuccessSound;
        }
        else
        {
            PredictedSpawnAtPosition(spliceProto.Seed1, Transform(ent).Coordinates);
            sound = ent.Comp.FailureSound;
        }
        if (_net.IsServer)
            _audio.PlayPvs(_audio.ResolveSound(sound), ent.Owner);
        UpdateState(ent);
    }

    [SubscribeLocalEvent]
    private void OnInsert(Entity<SeedSplicerComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        UpdateState(ent);
    }

    [SubscribeLocalEvent]
    private void OnEject(Entity<SeedSplicerComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        UpdateState(ent);
    }

    private SeedSplicePrototype? FindSplice(EntProtoId plantProto1, EntProtoId plantProto2)
    {
        var prototypes = _protoMan.EnumeratePrototypes<SeedSplicePrototype>();
        return prototypes.FirstOrDefault(p => p.Seed1 == plantProto1 && p.Seed2 == plantProto2);
    }

    public string? FindSpliceName(EntProtoId plantProto1, EntProtoId plantProto2)
    {
        return FindSpliceName(FindSplice(plantProto1, plantProto2));
    }

    private string? FindSpliceName(SeedSplicePrototype? proto)
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

    private void UpdateState(Entity<SeedSplicerComponent> ent)
    {
        if (!_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot1, out var slot1) || !_slot.TryGetSlot(ent.Owner, SeedSplicerComponent.Slot2, out var slot2))
            return;
        var entUid1 = slot1.ContainerSlot?.ContainedEntity;
        var entUid2 = slot2.ContainerSlot?.ContainedEntity;
        string? resultName = null;
        int? chance = null;
        GetSeedInfo(entUid1, out _, out var seedProto1, out var seedName1);
        GetSeedInfo(entUid2, out _, out var seedProto2, out var seedName2);
        if (seedProto1 is not null && seedProto2 is not null)
        {
            var resultProto = FindSplice(seedProto1.Value, seedProto2.Value);
            resultName = FindSpliceName(resultProto);
            chance = resultProto is null ? null : (int)(resultProto.Chance * 100);
        }
        var state = new SeedSplicerBoundUserInterfaceState(seedName1, seedName2, resultName, chance);
        _userInterface.SetUiState(ent.Owner, SeedSplicerUiKey.Key, state);
    }

    private void GetSeedInfo(EntityUid? seed, out EntProtoId? plantProto, out EntProtoId? seedProto, out string? seedName)
    {
        plantProto = null;
        seedProto = null;
        seedName = null;
        if (seed is null)
            return;
        if (TryComp<SeedComponent>(seed, out var seedComp))
            plantProto = seedComp.PlantProtoId;
        if (_protoMan.Resolve(plantProto, out var plant) &&
            plant.TryComp<PlantDataComponent>(out var plantData, _entityManager.ComponentFactory))
        {
            seedProto = plantData.PacketPrototype;
            seedName = plantData.Name;
        }
    }
}
