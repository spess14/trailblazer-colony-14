using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Blueprint;

[Serializable, NetSerializable]
public sealed class BluebenchBoundUserInterfaceState(HashSet<BluebenchResearchPrototype> availableResearchEntries, BluebenchResearchPrototype? activeProject) : BoundUserInterfaceState
{
    public HashSet<BluebenchResearchPrototype> AvailableResearchEntries { get; } = availableResearchEntries;
    public BluebenchResearchPrototype? ActiveProject { get; } = activeProject;
    public Dictionary<ProtoId<StackPrototype>, int> MaterialProgress = new();
    public Dictionary<string, int> ComponentProgress = new();
    public Dictionary<ProtoId<TagPrototype>, int> TagProgress = new();
}
