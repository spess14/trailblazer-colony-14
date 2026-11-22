using Content.Shared.Chemistry;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Botany.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SeedSplicerComponent : Component
{
    public static string Slot1 = "seed1";
    public static string Slot2 = "seed2";
}

[Serializable, NetSerializable]
public enum SeedSplicerUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class SeedSplicerBoundUserInterfaceState(
    //SeedSplicePrototype? resultProto,
    string? seedOneName,
    string? seedTwoName,
    string? resultName)
    : BoundUserInterfaceState
{
    //public SeedSplicePrototype? ResultProto = resultProto;
    public string? SeedOneName = seedOneName;
    public string? SeedTwoName = seedTwoName;
    public string? ResultName = resultName;
}

[Serializable, NetSerializable]
public sealed class SeedSplicerSpliceEvent : BoundUserInterfaceMessage;
