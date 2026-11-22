using Content.Shared._tc14.Botany.Components;
using Content.Shared.Containers.ItemSlots;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._tc14.Botany;

[UsedImplicitly]
public sealed class SeedSplicerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private SeedSplicerWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<SeedSplicerWindow>();
        _window.SeedOneSlotButton.OnPressed += _ => SendPredictedMessage(new ItemSlotButtonPressedEvent(SeedSplicerComponent.Slot1));
        _window.SeedTwoSlotButton.OnPressed += _ => SendPredictedMessage(new ItemSlotButtonPressedEvent(SeedSplicerComponent.Slot2));
        _window.SpliceButton.OnPressed += _ => SendPredictedMessage(new SeedSplicerSpliceEvent());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        var castState = (SeedSplicerBoundUserInterfaceState) state;
        _window?.UpdateState(castState);
    }
}
