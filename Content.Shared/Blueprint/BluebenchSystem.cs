using System.Linq;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.UserInterface;
using Robust.Client.GameObjects;
using Robust.Client.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Blueprint;

/// <summary>
/// This handles the bluebench bullshit
/// </summary>
public sealed partial class BluebenchSystem : EntitySystem
{
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BluebenchComponent, ExaminedEvent>(OnBluebenchExamined);
        SubscribeLocalEvent<BluebenchComponent, BoundUIOpenedEvent>(OnUIOpened);
        SubscribeLocalEvent<BluebenchComponent, InteractUsingEvent>(OnInteractUsingEvent);
        SubscribeLocalEvent<BluebenchComponent, ResearchProjectStartMessage>(OnResearchProjectStart);
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
            foreach (var (key, value) in researchProject.StackRequirements)
            {
                e.PushMarkup($"{value}x {key.Id}");
            }
            foreach (var (key, value) in researchProject.TagRequirements)
            {
                e.PushMarkup($"{value.Amount}x {key.Id}");
            }
            foreach (var (key, value) in researchProject.ComponentRequirements)
            {
                e.PushMarkup($"{value.Amount}x {key}");
            }
        }
    }

    private void OnInteractUsingEvent(EntityUid entity, BluebenchComponent component, InteractUsingEvent args)
    {
        if (args.Handled)
            return;
    }

    private void OnResearchProjectStart(EntityUid uid, BluebenchComponent component, ResearchProjectStartMessage args)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();

        if (!prototypeManager.TryIndex<BluebenchResearchPrototype>(args.Id, out var prototype))
            return;

        component.ActiveProject = prototype;
        UpdateUiState(uid, component);
    }

    private void UpdateUiState(EntityUid uid, BluebenchComponent component)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var prototypes = prototypeManager.EnumeratePrototypes<BluebenchResearchPrototype>().ToHashSet();
        var availablePrototypes = new HashSet<BluebenchResearchPrototype>(prototypes.Except(component.ResearchedPrototypes));

        _uiSystem.SetUiState(uid, BluebenchUiKey.Key, new BluebenchBoundUserInterfaceState(availablePrototypes, component.ActiveProject));
    }
}
