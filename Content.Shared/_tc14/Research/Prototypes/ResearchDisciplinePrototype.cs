using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._tc14.Research.Prototypes;

/// <summary>
/// Defines research disciplines - a category of research. Quite similar to TechDisciplinePrototype.
/// </summary>
[Prototype]
public sealed partial class ResearchDisciplinePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Player-facing name, color not included.
    /// </summary>
    [DataField(required: true)]
    public LocId Name;

    /// <summary>
    /// Color of the discipline.
    /// </summary>
    [DataField(required: true)]
    public Color Color;

    /// <summary>
    /// An icon used to visually represent the discipline in UI.
    /// </summary>
    [DataField("icon")]
    public SpriteSpecifier Icon = default!;

    /// <summary>
    /// Determines the display priority in menus, descending order.
    /// </summary>
    [DataField]
    public int Priority;
}
