using Content.Server.Administration;
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared._tc14.Skills.Systems;
using Content.Shared.Administration;
using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server._tc14.Skills.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed class SetSkillExperienceCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    public string Command => "setskillexp";
    public string Description => "Set skill experience of an entity in a given skill.";
    public string Help => $"Usage: {Command} <entityUid> <skillId> <number>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        EntityUid entity;

        switch (args.Length)
        {
            case 3:
                if (!NetEntity.TryParse(args[0], out var netEnt) || !_entManager.TryGetEntity(netEnt, out var uid))
                {
                    shell.WriteLine($"{args[0]} is not a valid entity uid.");
                    return;
                }

                if (!_entManager.EntityExists(uid))
                {
                    shell.WriteLine($"No entity exists with uid {uid}.");
                    return;
                }

                entity = uid.Value;

                if (!_protoManager.HasIndex<SkillPrototype>(args[1]))
                {
                    shell.WriteLine($"No skill prototype found with id {args[1]}.");
                    return;
                }

                if (!int.TryParse(args[2], out var amountInt))
                {
                    shell.WriteLine($"Invalid amount: {args[2]}.");
                    return;
                }

                _entManager.System<PlayerSkillsSystem>().SetSkillExperience(args[1], entity, FixedPoint2.New(amountInt));
                break;
            default:
                shell.WriteLine(Help);
                return;
        }
    }
}
