using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     If true, admin ghosts can load things around them.
    ///     Keep in mind that this may crash the server if the admin uses it in bad faith
    /// </summary>
    public static readonly CVarDef<bool> AdminGhostsLoadTerrain =
        CVarDef.Create("tc14.terrain.admin_load", false, CVar.SERVERONLY);

    /// <summary>
    ///     How much time (in seconds) must pass before a ghost can respawn?
    /// </summary>
    public static readonly CVarDef<float> RespawnCooldown =
        CVarDef.Create("tc14.respawn.cooldown", 600f, CVar.SERVER);
}
