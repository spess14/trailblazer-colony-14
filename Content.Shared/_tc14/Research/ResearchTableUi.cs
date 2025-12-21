using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Research;

[Serializable, NetSerializable]
public enum ResearchTableUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class ResearchTableState : BoundUserInterfaceState
{
    public ResearchTableState(){}
}
