using Content.Shared._tc14.Research.Prototypes;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Trigger.Components.Effects;

/// <summary>
/// Adds research points to ResearchPointSourceComponent when triggered.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AddResearchPointsOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Research points that are to be added to the entity.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<ResearchDisciplinePrototype>, int> Points = new();
}
