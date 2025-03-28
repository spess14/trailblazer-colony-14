using System.Linq;
using Content.Shared.Examine;
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
    }

    private void OnUIOpened(EntityUid uid, BluebenchComponent component, BoundUIOpenedEvent e)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();

        _uiSystem.SetUiState(uid, BluebenchUiKey.Key, new BluebenchBoundUserInterfaceState(prototypeManager.EnumeratePrototypes<BluebenchResearchPrototype>().ToHashSet(), component.ActiveProject));
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
}
