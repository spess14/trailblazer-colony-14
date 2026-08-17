using Content.Shared.EntityConditions;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._tc14.Research.Prototypes;

/// <summary>
/// Contains info about a set of items that must be submitted via the research kit to receive research points.
/// </summary>
[Prototype]
public sealed partial class ResearchBountyPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Rewarded points for completing the bounty.
    /// </summary>
    [DataField(required: true)]
    public Dictionary<ProtoId<ResearchDisciplinePrototype>, int> RewardedPoints = new();

    /// <summary>
    /// How many things must be submitted in order to complete the bounty?
    /// </summary>
    [DataField]
    public int MaxAmount = 1;

    /// <summary>
    /// Which conditions does the item(s) need to fulfill in order to count towards the bounty?
    /// </summary>
    [DataField]
    public EntityCondition[]? Conditions;

    /// <summary>
    /// The visible name of the bounty (describes the items you need to supply).
    /// </summary>
    [DataField]
    public LocId Name = string.Empty;

    /// <summary>
    /// Entproto used as an icon near the bounty name. Probably put the sprite of the most likely item to be submitted.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId SpriteEntity;
}
