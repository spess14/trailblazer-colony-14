using Content.Shared._tc14.Botany.Components;
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
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        var castState = (SeedSplicerBoundUserInterfaceState) state;
        _window?.UpdateState(castState);
    }
}
