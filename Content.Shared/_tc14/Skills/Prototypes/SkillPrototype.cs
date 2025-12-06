using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Skills.Prototypes;

/// <summary>
/// Skill prototype; how is it displayed, what values does it use for calculations, etc.
/// </summary>
[Prototype]
public sealed partial class SkillPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// Skill name, localized. This will be visible to the player.
    /// </summary>
    [DataField(required: true)]
    public LocId Name;

    /// <summary>
    /// Max skill level.
    /// </summary>
    [DataField]
    public FixedPoint2 MaxLevel = FixedPoint2.New(20);

    /// <summary>
    /// Determines the display priority in menus, descending order.
    /// </summary>
    [DataField]
    public int Priority;

    /// <summary>
    /// How much is gained when you gain skill experience?
    /// </summary>
    [DataField]
    public FixedPoint2 DeltaPoint = FixedPoint2.New(5); //=0.05

    /// <summary>
    /// Initial skill value by default.
    /// </summary>
    [DataField]
    public FixedPoint2 InitialValue = FixedPoint2.Zero;
}
