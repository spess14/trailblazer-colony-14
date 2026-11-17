using Content.Shared.Botany.Items.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Botany.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SeedSplicerComponent : Component
{

}

[Serializable, NetSerializable]
public enum SeedSplicerUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class SeedSplicerBoundUserInterfaceState(
    Entity<SeedComponent>? seedOne,
    Entity<SeedComponent>? seedTwo,
    string? seedOneName,
    string? seedTwoName)
    : BoundUserInterfaceState
{
    public Entity<SeedComponent>? SeedOne = seedOne;
    public Entity<SeedComponent>? SeedTwo = seedTwo;
    public string? SeedOneName = seedOneName;
    public string? SeedTwoName = seedTwoName;
}

