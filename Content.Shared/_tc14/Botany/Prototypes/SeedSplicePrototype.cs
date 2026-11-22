using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Botany.Prototypes;

[Prototype]
public sealed partial class SeedSplicePrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Input seed for the splicing.
    /// Note: order does actually matter here.
    /// </summary>
    [DataField]
    public EntProtoId Seed1;

    /// <summary>
    /// <see cref="Seed1"/>
    /// </summary>
    [DataField]
    public EntProtoId Seed2;

    /// <summary>
    /// The resulting seed of the splice.
    /// </summary>
    [DataField]
    public EntProtoId OutputSeed;

    /// <summary>
    /// Chance of the splice succeeding.
    /// Upon failure, return the first seed.
    /// </summary>
    [DataField]
    public FixedPoint2 Chance = FixedPoint2.New(0.5f);
}
