using Content.Shared._tc14.Research.Components;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._tc14.Research.UI.Kit;

[UsedImplicitly]
public sealed class ResearchKitBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private ResearchKitWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<ResearchKitWindow>();
        //TODO
    }

    public override void Update()
    {
        base.Update();

        if (_window is null || !EntMan.TryGetComponent(Owner, out ResearchKitComponent? comp))
            return;
        _window.UpdateState(comp.Bounties);
    }
}
