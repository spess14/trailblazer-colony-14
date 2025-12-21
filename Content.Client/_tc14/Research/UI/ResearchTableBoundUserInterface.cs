using Content.Shared._tc14.Research;
using Robust.Client.UserInterface;

namespace Content.Client._tc14.Research.UI;

public sealed class ResearchTableBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private ResearchTableWindow _window = default!;

    public ResearchTableBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<ResearchTableWindow>();
        _window.SetEntity(Owner);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not ResearchTableState)
            return;
    }
}
