using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Blueprint;

[Serializable, NetSerializable]
public sealed class BluebenchBoundUserInterfaceState(HashSet<ProtoId<BluebenchResearchPrototype>> availableResearchEntries, BluebenchResearchPrototype? activeProject,  Dictionary<ProtoId<StackPrototype>, int> materialProgress, Dictionary<string, int> componentProgress, Dictionary<ProtoId<TagPrototype>, int> tagProgress, HashSet<BluebenchResearchPrototype> researchedPrototypes) : BoundUserInterfaceState
{
    public HashSet<ProtoId<BluebenchResearchPrototype>> AvailableResearchEntries { get; } = availableResearchEntries;
    public BluebenchResearchPrototype? ActiveProject { get; } = activeProject;
    public Dictionary<ProtoId<StackPrototype>, int> MaterialProgress = materialProgress;
    public Dictionary<string, int> ComponentProgress = componentProgress;
    public Dictionary<ProtoId<TagPrototype>, int> TagProgress = tagProgress;
    public HashSet<BluebenchResearchPrototype> ResearchedPrototypes = researchedPrototypes;
}
