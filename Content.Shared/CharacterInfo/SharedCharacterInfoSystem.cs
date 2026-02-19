using Content.Shared._Starlight.CollectiveMind; // Starlight - Collective Minds
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Objectives;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo;

[Serializable, NetSerializable]
public sealed class RequestCharacterInfoEvent : EntityEventArgs
{
    public readonly NetEntity NetEntity;

    public RequestCharacterInfoEvent(NetEntity netEntity)
    {
        NetEntity = netEntity;
    }
}

// TC14: added skills info
[Serializable, NetSerializable]
public sealed class CharacterInfoEvent : EntityEventArgs
{
    public readonly NetEntity NetEntity;
    public readonly string JobTitle;
    public readonly Dictionary<string, List<ObjectiveInfo>> Objectives;
    public readonly string? Briefing;
    public readonly Dictionary<CollectiveMindPrototype, CollectiveMindMemberData>? CollectiveMinds; // Starlight - Collective Minds
    public readonly Dictionary<ProtoId<SkillPrototype>, FixedPoint2> Skills;

    public CharacterInfoEvent(NetEntity netEntity,
        string jobTitle,
        Dictionary<string, List<ObjectiveInfo>> objectives,
        string? briefing,
        Dictionary<CollectiveMindPrototype, CollectiveMindMemberData>? collectiveMinds,
        Dictionary<ProtoId<SkillPrototype>, FixedPoint2> skills) // Starlight - Collective Minds
    {
        NetEntity = netEntity;
        JobTitle = jobTitle;
        Objectives = objectives;
        Briefing = briefing;
        CollectiveMinds = collectiveMinds; // Starlight - Collective Minds
        Skills = skills;
    }
}
