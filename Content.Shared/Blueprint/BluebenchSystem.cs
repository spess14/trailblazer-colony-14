using System.Linq;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Materials;
using Content.Shared.Research.Components;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared.Blueprint;

/// <summary>
/// This handles the bluebench bullshit
/// </summary>
public sealed class BluebenchSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedStackSystem _stackSystem = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;
    [Dependency] private readonly IComponentFactory _factory = default!;
    [Dependency] private readonly SharedMaterialStorageSystem _material = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BluebenchComponent, ExaminedEvent>(OnBluebenchExamined);
        SubscribeLocalEvent<BluebenchComponent, BoundUIOpenedEvent>(OnUIOpened);
        SubscribeLocalEvent<BluebenchComponent, InteractUsingEvent>(OnInteractUsingEvent);
        SubscribeLocalEvent<BluebenchComponent, ResearchProjectStartMessage>(OnResearchProjectStart);
        SubscribeLocalEvent<BluebenchComponent, MaterialEntityInsertedEvent>(OnMaterialInserted);
    }

    private void OnMaterialInserted(EntityUid uid, BluebenchComponent component, MaterialEntityInsertedEvent args)
    {
        UpdateUiState(uid, component);
    }

    private void OnUIOpened(EntityUid uid, BluebenchComponent component, BoundUIOpenedEvent e)
    {
        UpdateUiState(uid, component);
    }

    private void OnBluebenchExamined(EntityUid entity, BluebenchComponent component, ExaminedEvent e)
    {
        if (!e.IsInDetailsRange)
            return;

        if (component.ActiveProject is null)
            return;

        var researchProject = component.ActiveProject;

        using (e.PushGroup(nameof(BluebenchComponent)))
        {
            e.PushMarkup($"Currently researching: {researchProject.Name}");
            foreach (var (key, value) in component.MaterialProgress)
            {
                if (value == 0)
                    continue;

                e.PushMarkup($"{value}x {key.Id}");
            }

            foreach (var (key, value) in component.TagProgress)
            {
                if (value == 0)
                    continue;

                e.PushMarkup($"{value}x {key.Id}");
            }

            foreach (var (key, value) in component.ComponentProgress)
            {
                if (value == 0)
                    continue;

                e.PushMarkup($"{value}x {key}");
            }
        }
    }

    private bool TryInsertStack(EntityUid used, BluebenchComponent component, StackComponent stack)
    {
        var type = stack.StackTypeId;

        if (component.ActiveProject is null)
            return false;

        if (!component.ActiveProject.StackRequirements.ContainsKey(type))
            return false;

        if (component.MaterialProgress[type] == 0)
            return false;

        var stackCount = _stackSystem.GetCount(used, stack);
        var toInsert = Math.Clamp(component.MaterialProgress[type], 1, stackCount);

        component.MaterialProgress[type] -= toInsert;
        _stackSystem.SetCount(used, stackCount - toInsert, stack);

        return true;
    }

    private void OnInteractUsingEvent(EntityUid entity, BluebenchComponent component, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (component.ActiveProject is null)
            return;

        if (TryComp<StackComponent>(args.Used, out var stack))
        {
            if (TryInsertStack(args.Used, component, stack))
                args.Handled = true;
        }

        if (TryComp<TagComponent>(args.Used, out var tagComp))
        {
            foreach (var (tagName, _) in component.ActiveProject.TagRequirements)
            {
                if (component.TagProgress[tagName] == 0)
                    continue;

                if (!_tagSystem.HasTag(tagComp, tagName))
                    continue;

                if (!args.Handled)
                {
                    QueueDel(args.Used);
                }

                component.TagProgress[tagName]--;
                args.Handled = true;
            }
        }

        foreach (var (compName, _) in component.ActiveProject.ComponentRequirements)
        {
            if (component.ComponentProgress[compName] == 0)
                continue;

            var registration = _factory.GetRegistration(compName);

            if (!HasComp(args.Used, registration.Type))
                continue;

            // Insert the entity, if it hasn't already been inserted
            if (!args.Handled)
            {
                QueueDel(args.Used);
                args.Handled = true;
            }

            component.ComponentProgress[compName]--;
        }

        if (IsComplete(component))
        {
            var result = Spawn("BaseBlueprint", Transform(entity).Coordinates);
            GenerateBlueprint(result, component.ActiveProject);

            component.ComponentProgress.Clear();
            component.MaterialProgress.Clear();
            component.TagProgress.Clear();
            component.ResearchedPrototypes.Add(component.ActiveProject);
            component.ActiveProject = null;
            RecomputeAvailable(component);
        }

        UpdateUiState(entity, component);
    }

    private void GenerateBlueprint(EntityUid uid, BluebenchResearchPrototype project)
    {
        var blueprint = AddComp<BlueprintComponent>(uid);
        foreach (var recipe in project.OutputRecipes)
        {
            blueprint.ProvidedRecipes.Add(recipe);
        }

        if (project.OutputPacks is null)
            return;

        foreach (var pack in project.OutputPacks)
        {
            if (!_prototypeManager.TryIndex(pack, out var proto))
                continue;

            foreach (var recipe in proto.Recipes)
            {
                blueprint.ProvidedRecipes.Add(recipe);
            }
        }

        var tagComp = AddComp<TagComponent>(uid);

        foreach (var tag in project.OutputTags) // afaik there's   no prettier way to do this, + it all comes down to this. sorry
        {
            _tagSystem.AddTag(uid, tag);
        }
    }

    private void OnResearchProjectStart(EntityUid uid, BluebenchComponent component, ResearchProjectStartMessage args)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();

        if (!prototypeManager.TryIndex<BluebenchResearchPrototype>(args.Id, out var prototype))
            return;

        if (_material.GetMaterialAmount(uid, "Paper") == 0)
            return;

        if (!_material.TryChangeMaterialAmount(uid, "Paper", -1))
            return;

        if (component.ResearchedPrototypes.Contains(prototype))
        {
            var result = Spawn("BaseBlueprint", Transform(uid).Coordinates);
            GenerateBlueprint(result, prototype);

            UpdateUiState(uid, component);
            return;
        }

        if (!component.AvailablePrototypes.Contains(prototype))
            return;

        if (component.ActiveProject != null)
            return;

        component.ActiveProject = prototype;
        foreach (var requirement in prototype.ComponentRequirements)
        {
            component.ComponentProgress.Add(requirement.Key, requirement.Value.Amount);
        }

        foreach (var requirement in prototype.TagRequirements)
        {
            component.TagProgress.Add(requirement.Key, requirement.Value.Amount);
        }

        foreach (var requirement in prototype.StackRequirements)
        {
            component.MaterialProgress.Add(requirement.Key, requirement.Value);
        }

        UpdateUiState(uid, component);
    }

    private void UpdateUiState(EntityUid uid, BluebenchComponent component)
    {
        _uiSystem.SetUiState(uid,
            BluebenchUiKey.Key,
            new BluebenchBoundUserInterfaceState(component.AvailablePrototypes,
                component.ActiveProject,
                component.MaterialProgress,
                component.ComponentProgress,
                component.TagProgress,
                _material.GetMaterialAmount(uid, "Paper"),
                component.ResearchedPrototypes));
    }

    private void RecomputeAvailable(BluebenchComponent component)
    {
        var prototypes = _prototypeManager.EnumeratePrototypes<BluebenchResearchPrototype>().ToHashSet();
        component.AvailablePrototypes.Clear();

        foreach (var proto in prototypes.Where(proto => !component.ResearchedPrototypes.Contains(proto)))
        {
            if (proto.RequiredResearch != null)
            {
                var flag = true;

                foreach (var reqProtoId in proto.RequiredResearch)
                {
                    if (_prototypeManager.TryIndex(reqProtoId, out var reqProto) &&
                        component.ResearchedPrototypes.Contains(reqProto))
                        continue;

                    flag = false;
                    break;
                }

                if (!flag)
                    continue;
            }

            component.AvailablePrototypes.Add(proto);
        }
    }

    private static bool IsComplete(BluebenchComponent component)
    {
        if (component.ActiveProject is null)
            return false;

        foreach (var (type, _) in component.ActiveProject.StackRequirements)
        {
            if (component.MaterialProgress[type] != 0)
                return false;
        }

        foreach (var (compName, _) in component.ActiveProject.ComponentRequirements)
        {
            if (component.ComponentProgress[compName] != 0)
                return false;
        }

        foreach (var (tagName, _) in component.ActiveProject.TagRequirements)
        {
            if (component.TagProgress[tagName] != 0)
                return false;
        }

        return true;
    }
}
