using Content.Server.Administration;
using Content.Shared._tc14.Skills.Components;
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.Administration;
using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._tc14.Skills.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed partial class AvgSkillsCommand : IConsoleCommand
{
    [Dependency] private IEntityManager _entManager = default!;

    public string Command => "avgskills";
    public string Description => "View average skills of everyone on the server.";
    public string Help => $"Usage: {Command}";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        switch (args.Length)
        {
            case 0:
                var players = 0;
                var totalSkills = new Dictionary<ProtoId<SkillPrototype>, FixedPoint2>();
                var query = _entManager.AllEntityQueryEnumerator<PlayerSkillsComponent, ActorComponent>();
                while (query.MoveNext(out var comp, out _))
                {
                    players++;
                    var skills = comp.Skills;
                    if (skills.Count == 0)
                        continue;
                    foreach (var pair in skills)
                    {
                        if (totalSkills.ContainsKey(pair.Key))
                        {
                            totalSkills[pair.Key] += pair.Value;
                        }
                        else
                        {
                            totalSkills.Add(pair.Key, pair.Value);
                        }
                    }
                }
                if (players == 0)
                {
                    shell.WriteLine("No players with skills found!");
                    return;
                }
                foreach (var pair in totalSkills)
                {
                    shell.WriteLine($"{pair.Key}: {pair.Value/players}");
                }
                break;
            default:
                shell.WriteLine(Help);
                return;
        }
    }
}
