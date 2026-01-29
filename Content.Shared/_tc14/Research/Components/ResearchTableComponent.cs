using Content.Shared._tc14.Research.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._tc14.Research.Components;

/// <summary>
/// Used for a research table; stores researched techs and discipline points.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ResearchTableComponent : Component
{
    /// <summary>
    /// Discipline points that are stored inside the table.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<ResearchDisciplinePrototype>, int> StoredPoints = new();

    /// <summary>
    /// List of technologies that are researched and can be printed.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<ProtoId<ResearchEntryPrototype>> ResearchedTechs = [];

    /// <summary>
    /// Time at which the table will be able to make another blueprint.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextPrintTime = TimeSpan.Zero;

    /// <summary>
    /// The time between prints.
    /// </summary>
    [DataField]
    public TimeSpan PrintDelay = TimeSpan.FromSeconds(5);
}
