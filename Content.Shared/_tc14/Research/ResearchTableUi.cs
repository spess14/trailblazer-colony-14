using Content.Shared._tc14.Research.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Research;

[Serializable, NetSerializable]
public enum ResearchTableUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class ResearchTableTechResearchedMessage : BoundUserInterfaceMessage
{
    public ProtoId<ResearchEntryPrototype> Id;

    public ResearchTableTechResearchedMessage(ProtoId<ResearchEntryPrototype> id)
    {
        Id = id;
    }
}

[Serializable, NetSerializable]
public sealed class ResearchTablePrintBlueprint : BoundUserInterfaceMessage
{
    public ProtoId<ResearchEntryPrototype> Id;

    public ResearchTablePrintBlueprint(ProtoId<ResearchEntryPrototype> id)
    {
        Id = id;
    }
}

[Serializable, NetSerializable]
public sealed class ResearchTableState : BoundUserInterfaceState
{
    public Dictionary<ProtoId<ResearchDisciplinePrototype>, int> StoredPoints;
    public HashSet<ProtoId<ResearchEntryPrototype>> ResearchedTechs;

    public ResearchTableState(Dictionary<ProtoId<ResearchDisciplinePrototype>, int> storedPoints,
        HashSet<ProtoId<ResearchEntryPrototype>> researchedTechs)
    {
        StoredPoints = storedPoints;
        ResearchedTechs = researchedTechs;
    }
}
