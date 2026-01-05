using Content.Shared._tc14.Research.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Research.Components;

/// <summary>
/// Added to items that provide points to research table when used.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
// ReSharper disable once InconsistentNaming
public sealed partial class TCResearchPointSourceComponent : Component
{
    /// <summary>
    /// Discipline points stored.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<ResearchDisciplinePrototype>, int> StoredPoints = new();

    /// <summary>
    /// Do we show in detail the points stored in the object?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool HasExamine;

    /// <summary>
    /// Does this item need to be examined using an observation kit first?
    /// Used for research prototypes, since those need to go through a researcher first.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool RequiresExamination;
}
