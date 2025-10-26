using Content.Shared._tc14.Skills.Components;
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared._tc14.Skills.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class PlayerSkillsSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _protoMan = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSkillsComponent, ComponentInit>(OnInit);
    }

    private void OnInit(EntityUid uid, PlayerSkillsComponent component, ref ComponentInit args)
    {
        var existingSkills = _protoMan.EnumeratePrototypes<SkillPrototype>();
        foreach (var skillProto in existingSkills)
        {
            component.Skills.Add(skillProto.ID, skillProto.InitialValue);
        }
    }

    /// <summary>
    /// Returns the dictionary of skills. Use if you need all the skill data at once.
    /// </summary>
    public Dictionary<ProtoId<SkillPrototype>, FixedPoint2>? GetSkills(EntityUid uid)
    {
        return !TryComp<PlayerSkillsComponent>(uid, out var comp) ? null : comp.Skills;
    }

    /// <summary>
    /// Get the experience in a specific skill.
    /// </summary>
    public FixedPoint2? GetSkill(ProtoId<SkillPrototype> skillId, EntityUid uid)
    {
        return GetSkills(uid)?[skillId];
    }

    /// <summary>
    /// Directly set experience in a skill to a value. This must be used instead of directly setting it on a component.
    /// </summary>
    public void SetSkillExperience(ProtoId<SkillPrototype> skillId, EntityUid uid, FixedPoint2 value)
    {
        if (!TryComp<PlayerSkillsComponent>(uid, out var comp))
            return;
        if (!_protoMan.Resolve(skillId, out var prototype))
            return;
        comp.Skills[skillId] = Math.Clamp((float)(comp.Skills[skillId]+value), 0f, prototype.MaxLevel);
    }

    /// <summary>
    /// Adds experience to the skill.
    /// </summary>
    public void AddSkillExperience(ProtoId<SkillPrototype> skillId, EntityUid uid, FixedPoint2 delta)
    {
        if (!_protoMan.Resolve(skillId, out var skillPrototype))
        {
            return;
        }

        var skills = GetSkills(uid);
        if (skills is null)
            return;
        var skillExp = (float)skills[skillId];
        SetSkillExperience(skillId, uid, skillExp+delta);
    }
}
