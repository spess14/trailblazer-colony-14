using Robust.Shared.Serialization;

namespace Content.Shared.Blueprint;

[NetSerializable, Serializable]
public enum BluebenchUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class ResearchProjectStartMessage(string id) : BoundUserInterfaceMessage
{
    public string Id = id;
}
