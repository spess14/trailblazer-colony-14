using Robust.Shared.Configuration;

namespace Content.Shared._tc14.CCVar;

[CVarDefs]
public sealed partial class CCVars
{
    public static readonly CVarDef<int> MaxPassionPoints =
        CVarDef.Create("tc14.passions.max_points", 5, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<int> MaxPointsPerSkill =
        CVarDef.Create("tc14.passions.max_points_per_skill", 3, CVar.SERVER | CVar.REPLICATED);
}
