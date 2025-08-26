using Content.Shared.Blueprint;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Blueprint.UI;

[UsedImplicitly]
public sealed class BluebenchBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private BluebenchMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<BluebenchMenu>();

        _menu.OnTechnologyProjectStart += id =>
        {
            SendMessage(new ResearchProjectStartMessage(id));
        };
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        _menu?.Dispose();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is not BluebenchBoundUserInterfaceState newState)
            return;

        if (_menu == null)
            return;

        _menu.ActiveResearchProto = newState.ActiveProject;
        _menu.MaterialProgress = newState.MaterialProgress;
        _menu.TagProgress = newState.TagProgress;
        _menu.ComponentProgress = newState.ComponentProgress;
        _menu.ResearchedPrototypes = newState.ResearchedPrototypes;

        _menu?.UpdateResearchEntries(newState.AvailableResearchEntries);
        _menu?.UpdateRequiredComponents();
    }
}
