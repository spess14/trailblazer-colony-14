using Content.Shared._tc14.Tools.Components;
using Content.Shared._tc14.Tools.Systems;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Kitchen.Components;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server._tc14.Tools.Systems;

/// <summary>
/// Handles GrindOnDoAfterComponent
/// </summary>
public sealed class GrindOnDoAfterSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly SharedPopupSystem _popups = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GrindOnDoAfterComponent, GrindDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, GrindOnDoAfterComponent component, ref GrindDoAfterEvent args)
    {
        if (args.Cancelled)
            return;
        var item = _itemSlots.GetItemOrNull(uid, component.ItemSlot);
        if (item is null || !TryComp<SolutionContainerManagerComponent>(item, out var solutions) || !HasComp<ContainerManagerComponent>(item))
            return;
        if (!TryComp<ExtractableComponent>(item, out var extractable) || extractable.GrindableSolution is null ||
            !_solution.TryGetSolution(new Entity<SolutionContainerManagerComponent?>(item.Value, solutions), extractable.GrindableSolution, out var solutionEnt))
            return;

        var solution = solutionEnt.Value.Comp.Solution;
        _solution.TryGetSolution(uid, component.Solution, out var target);
        _solution.TryAddSolution(target!.Value, solution);

        QueueDel(item);
        _popups.PopupClient(Loc.GetString("grind-grinded"), args.User);
    }
}
