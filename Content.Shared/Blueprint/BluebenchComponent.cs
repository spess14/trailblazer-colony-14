using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Blueprint;

/// <summary>
/// Used for the blueprint workbench
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(BluebenchSystem))]
[AutoGenerateComponentState]
public sealed partial class BluebenchComponent : Component
{
    [DataField, AutoNetworkedField]
    public BluebenchResearchPrototype? ActiveProject;

    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<StackPrototype>, int> MaterialProgress = new();

    [DataField, AutoNetworkedField]
    public Dictionary<string, int> ComponentProgress = new();

    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<TagPrototype>, int> TagProgress = new();

    [DataField, AutoNetworkedField]
    public HashSet<BluebenchResearchPrototype> ResearchedPrototypes = [];

    [DataField, AutoNetworkedField]
    public HashSet<ProtoId<BluebenchResearchPrototype>> AvailablePrototypes = [];
}
