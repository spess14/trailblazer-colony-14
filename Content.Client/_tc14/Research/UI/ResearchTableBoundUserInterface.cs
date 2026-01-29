using Content.Shared._tc14.Research;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._tc14.Research.UI;

[UsedImplicitly]
public sealed class ResearchTableBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private ResearchTableWindow? _window = default!;

    public ResearchTableBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<ResearchTableWindow>();
        _window.SetEntity(Owner);

        _window.OnResearchButtonClicked += id =>
        {
            SendPredictedMessage(new ResearchTableTechResearchedMessage(id));
        };
        _window.OnPrintButtonClicked += id =>
        {
            SendPredictedMessage(new ResearchTablePrintBlueprint(id));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not ResearchTableState rState)
            return;
        _window?.UpdateState(rState);
    }
}
