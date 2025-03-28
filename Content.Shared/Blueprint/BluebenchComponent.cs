using Robust.Shared.GameStates;

namespace Content.Shared.Blueprint;

/// <summary>
/// Used for the blueprint workbench
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(BluebenchSystem))]
[AutoGenerateComponentState]
public sealed partial class BluebenchComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    public BluebenchResearchPrototype? ActiveProject;
}
