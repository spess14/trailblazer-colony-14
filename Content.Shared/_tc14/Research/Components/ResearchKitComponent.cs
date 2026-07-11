using Content.Shared._tc14.Research.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Research.Components;

/// <summary>
/// This component is added to research kits - tools that give you bounties to conduct research.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ResearchKitComponent : Component
{
    /// <summary>
    /// Acquired points will be multiplied by this (as well as the user's research skill multiplier)
    /// </summary>
    [DataField, AutoNetworkedField]
    public FixedPoint2 PointsMultiplier = 1;

    /// <summary>
    /// Bounty entries the research kit currentry has.
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<ResearchKitBounty> Bounties = new();
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class ResearchKitBounty
{
    /// <summary>
    /// Is this bounty blocked from being rerolled?
    /// </summary>
    [DataField]
    public bool IsLocked;

    [DataField(required: true)]
    public ProtoId<ResearchBountyPrototype> BountyPrototype;
}
