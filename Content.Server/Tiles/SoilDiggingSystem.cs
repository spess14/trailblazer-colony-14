using Content.Shared._tc14.Tiles;

namespace Content.Server.Tiles;

/// <summary>
/// This handles...
/// </summary>
public sealed class SoilDiggingSystem : EntitySystem
{
    [Dependency] private readonly EntityManager _entMan = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<SoilDigEvent>(OnDoAfter);
    }

    private void OnDoAfter(SoilDigEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        _entMan.SpawnEntity(args.SoilPrototypeName, Transform(args.User).Coordinates);

        args.Handled = true;
    }
}
