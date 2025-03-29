using Robust.Shared.Serialization;

namespace Content.Shared.Blueprint;

[Serializable, NetSerializable]
public sealed class BluebenchBoundUserInterfaceState(HashSet<BluebenchResearchPrototype> availableResearchEntries, BluebenchResearchPrototype? activeProject) : BoundUserInterfaceState
{
    public HashSet<BluebenchResearchPrototype> AvailableResearchEntries { get; } = availableResearchEntries;
    public BluebenchResearchPrototype? ActiveProject { get; } = activeProject;
}
