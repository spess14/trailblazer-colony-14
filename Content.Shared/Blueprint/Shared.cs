using Robust.Shared.Serialization;

namespace Content.Shared.Blueprint;

[NetSerializable, Serializable]
public enum BluebenchUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class ResearchProjectStartMessage : BoundUserInterfaceMessage
{
    public string Id;

    public ResearchProjectStartMessage(string id)
    {
        Id = id;
    }
}
