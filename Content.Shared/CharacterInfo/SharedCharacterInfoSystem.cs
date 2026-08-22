using Content.Shared._Starlight.CollectiveMind; // Starlight - Collective Minds
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Objectives;
using Content.Shared.Roles;
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
    public readonly ProtoId<JobPrototype>? Job;
    public readonly Dictionary<string, List<ObjectiveInfo>> Objectives;
    public readonly string? Briefing;
    public readonly Dictionary<ProtoId<CollectiveMindPrototype>, CollectiveMindMemberData>? CollectiveMinds; // Starlight - Collective Minds
    public readonly Dictionary<ProtoId<SkillPrototype>, FixedPoint2> Skills;

    public CharacterInfoEvent(NetEntity netEntity, Dictionary<string, List<ObjectiveInfo>> objectives, string? briefing, ProtoId<JobPrototype>? job, Dictionary<ProtoId<CollectiveMindPrototype>, CollectiveMindMemberData>? collectiveMinds,
        Dictionary<ProtoId<SkillPrototype>, FixedPoint2> skills) // Starlight - Collective Minds
    {
        NetEntity = netEntity;
        Objectives = objectives;
        Briefing = briefing;
        Job = job;
        CollectiveMinds = collectiveMinds; // Starlight - Collective Minds
        Skills = skills;
    }
}
