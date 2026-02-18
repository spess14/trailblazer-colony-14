using System.Linq;
using Content.Shared._tc14.CCVar;
using Content.Shared._tc14.Skills.Components;
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._tc14.Skills.Systems;

/// <summary>
/// Has basic API for player skills.
/// </summary>
public sealed class PlayerSkillsSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;

    private int _skillPointsAmount;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerSkillsComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<PlayerSkillsComponent, PlayerSpawnCompleteEvent>(OnPlayerSpawn);

        _skillPointsAmount = _configurationManager.GetCVar(CCVars.RoundstartSkillPoints);
    }

    private void OnPlayerSpawn(EntityUid uid, PlayerSkillsComponent component, ref PlayerSpawnCompleteEvent args)
    {
        var passions = args.Profile.Passions;
        var passionList = new List<ProtoId<SkillPrototype>>();
        var skills = GetSkills(uid);
        if (skills is null)
            return;
        foreach (var skillProto in skills.Keys)
        {
            passions.TryGetValue(skillProto, out var passionPoints);
            passionList.AddRange(Enumerable.Repeat(skillProto, passionPoints + 1));
        }
        for (var i = 0; i < _skillPointsAmount; i++)
        {
            AddSkillExperience(_random.Pick(passionList), uid, FixedPoint2.New(1));
        }
    }

    private void OnInit(EntityUid uid, PlayerSkillsComponent component, ref ComponentInit args)
    {
        var existingSkills = _protoMan.EnumeratePrototypes<SkillPrototype>();
        existingSkills = existingSkills.OrderByDescending(s => s.Priority);
        foreach (var skillProto in existingSkills)
        {
            if (!component.Skills.ContainsKey(skillProto.ID))
                component.Skills.Add(skillProto.ID, skillProto.InitialValue);
        }
        Dirty(uid, component);
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
        comp.Skills[skillId] = FixedPoint2.Clamp(value, FixedPoint2.Zero, prototype.MaxLevel);
        Dirty(uid, comp);
    }

    /// <summary>
    /// Adds experience to the skill.
    /// </summary>
    public void AddSkillExperience(ProtoId<SkillPrototype> skillId, EntityUid uid, FixedPoint2 delta)
    {
        if (!_protoMan.Resolve(skillId, out _))
        {
            return;
        }

        var skills = GetSkills(uid);
        if (skills is null)
            return;
        var skillExp = skills[skillId];
        SetSkillExperience(skillId, uid, skillExp+delta);
    }

    /// <summary>
    /// Use this for any roll/contest
    /// </summary>
    public int GetSkillLevel(ProtoId<SkillPrototype> skillId, EntityUid uid)
    {
        var skillExp = GetSkill(skillId, uid);
        if (skillExp is null)
            return 0;
        return (int)Math.Floor(skillExp.Value.Float());
    }

    /// <summary>
    /// Makes a DC check for a certain skill. The type of check is dN + skill.
    /// </summary>
    public int MakeRoll(ProtoId<SkillPrototype> skillId, EntityUid uid, int d)
    {
        var rand = new System.Random((int)_timing.CurTick.Value);
        return rand.Next(1, d) + GetSkillLevel(skillId, uid);
    }

    /// <summary>
    /// Returns the result of rolling the chance, respecting overflow chance values. 250% will return 2-3, 400% will return 4, etc.
    /// Good example of this system: https://wiki.hypixel.net/Ferocity
    /// TODO This does not belong here. Move it to some sort of random wrapper.
    /// </summary>
    public int CumulativeChanceRoll(float chance, int cap = int.MaxValue)
    {
        var ret = 0;
        while (chance >= 1)
        {
            chance -= 1f;
            ret += 1;
        }
        if (_random.Prob(chance))
            ret += 1;
        return Math.Clamp(ret, 0, cap);
    }

    /// <summary>
    /// Makes a contest check between 2 players.
    /// If results are equal, return null.
    /// Otherwise, return if the first player won.
    /// </summary>
    public bool? Contest(ProtoId<SkillPrototype> skillId, EntityUid first, EntityUid second)
    {
        var firstRoll = MakeRoll(skillId, first, 20);
        var secondRoll = MakeRoll(skillId, second, 20);
        if (firstRoll == secondRoll)
            return null;
        return firstRoll > secondRoll;
    }

    public LocId GetVerbalLevelDesc(FixedPoint2 exp)
    {
        var lvl = (int)Math.Floor(exp.Float());
        return lvl switch
        {
            0 => new LocId("skills-0"),
            <= 4 => new LocId("skills-1to4"),
            <= 8 => new LocId("skills-5to8"),
            <= 12 => new LocId("skills-9to12"),
            <= 16 => new LocId("skills-13to16"),
            <= 19 => new LocId("skills-17to19"),
            20 => new LocId("skills-20"),
            _ => new LocId("skills-unknown"),
        };
    }
}
