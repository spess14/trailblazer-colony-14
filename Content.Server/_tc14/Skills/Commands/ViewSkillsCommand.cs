using Content.Server.Administration;
using Content.Shared._tc14.Skills.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server._tc14.Skills.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed class ViewSkillsCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    public string Command => "viewskills";
    public string Description => "View skills of a given entity.";
    public string Help => $"Usage: {Command} <entityUid>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        EntityUid entity;

        switch (args.Length)
        {
            case 0:
                if (player == null)
                {
                    shell.WriteLine("Only a player can run this command without arguments.");
                    return;
                }
                if (player.AttachedEntity == null)
                {
                    shell.WriteLine("You don't have an entity to view skills for.");
                    return;
                }

                entity = player.AttachedEntity.Value;
                CheckSkills(shell, entity);
                break;
            case 1:
                if (NetEntity.TryParse(args[0], out var uidNet) && _entManager.TryGetEntity(uidNet, out var uid))
                {
                    if (!_entManager.EntityExists(uid))
                    {
                        shell.WriteLine($"No entity found with uid {uid}");
                        return;
                    }
                    entity = uid.Value;
                    CheckSkills(shell, entity);
                }
                break;
            default:
                shell.WriteLine(Help);
                return;
        }
    }

    private void CheckSkills(IConsoleShell shell, EntityUid entity)
    {
        var skills = _entManager.System<PlayerSkillsSystem>().GetSkills(entity);
        if (skills is null)
        {
            shell.WriteLine("No skills found!");
            return;
        }
        foreach (var pair in skills)
        {
            shell.WriteLine($"{pair.Key}: {pair.Value}");
        }
    }
}
