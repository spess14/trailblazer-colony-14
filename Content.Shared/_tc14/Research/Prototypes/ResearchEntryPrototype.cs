using Content.Shared.Research.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Research.Prototypes;

/// <summary>
/// Defines a researchable technology in the research workbench.
/// </summary>
[Prototype]
public sealed partial class ResearchEntryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Player-facing name. Non-colored, since the color comes from discipline.
    /// </summary>
    [DataField]
    public LocId Name;

    /// <summary>
    /// Entity prototype of the research.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId IconPrototype;

    /// <summary>
    /// The research discipline this research belongs to. Also defines the type of research points you'll need.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ResearchDisciplinePrototype> Discipline;

    /// <summary>
    /// How many points do you need to research this?
    /// </summary>
    [DataField]
    public int Points = 100;

    /// <summary>
    /// Where on the research tree grid should this entry be located?
    /// </summary>
    [DataField]
    public Vector2i GridLocation = Vector2i.Zero;

    /// <summary>
    /// Which researches need to be done first before this one opens?
    /// </summary>
    [DataField]
    public HashSet<ProtoId<ResearchEntryPrototype>> Dependencies = new();

    /// <summary>
    /// What recipes does this research unlock?
    /// </summary>
    [DataField]
    public HashSet<ProtoId<LatheRecipePrototype>> UnlockedRecipes = new();
}
