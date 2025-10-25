using Content.Shared._tc14.Skills.Components;
using Content.Shared._tc14.Skills.Prototypes;
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
}
