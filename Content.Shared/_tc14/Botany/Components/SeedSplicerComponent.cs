using Content.Shared.Botany.Items.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
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
    EntProtoId? seedOneProto,
    EntProtoId? seedTwoProto,
    string? seedOneName,
    string? seedTwoName)
    : BoundUserInterfaceState
{
    public EntProtoId? SeedOneProto = seedOneProto;
    public EntProtoId? SeedTwoProto = seedTwoProto;
    public string? SeedOneName = seedOneName;
    public string? SeedTwoName = seedTwoName;
}

