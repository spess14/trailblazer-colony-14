using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Skills.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlayerSkillsComponent : Component
{
    /// <summary>
    /// Skill levels and progress towards them.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<SkillPrototype>, FixedPoint2> Skills = new();
}
