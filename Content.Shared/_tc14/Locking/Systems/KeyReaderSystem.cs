using System.Diagnostics.CodeAnalysis;
using Content.Shared._tc14.Locking.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Lock;
using JetBrains.Annotations;

namespace Content.Shared._tc14.Locking.Systems;

/// <summary>
/// Handles reading from a physical key.
/// </summary>
public sealed partial class KeyReaderSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KeyReaderComponent, FindAvailableLocksEvent>(OnFindAvailableLocks);
        SubscribeLocalEvent<KeyReaderComponent, CheckUserHasLockAccessEvent>(OnCheckLockAccess);
    }

    private void OnFindAvailableLocks(Entity<KeyReaderComponent> ent, ref FindAvailableLocksEvent args)
    {
        args.FoundReaders |= LockTypes.Key;
    }

    private void OnCheckLockAccess(Entity<KeyReaderComponent> ent, ref CheckUserHasLockAccessEvent args)
    {
        if (!args.FoundReaders.HasFlag(LockTypes.Key))
            return;

        if (IsAllowed(ent.Owner, args.User, out var denyReason))
            args.HasAccess |= LockTypes.Key;
        else
            args.DenyReason = denyReason;
    }

    [PublicAPI]
    public bool IsAllowed(Entity<KeyReaderComponent?> target,
        EntityUid user,
        [NotNullWhen(false)] out string? denyReason)
    {
        denyReason = null;
        if (!Resolve(target, ref target.Comp, false))
            return true;

        var item = _hands.GetActiveItem(user);

        if (item is null || !TryComp<PhysicalKeyComponent>(item, out var keyComp))
        {
            denyReason = Loc.GetString("lockkey-no-key");
            //_popup.PopupClient(denyReason, target, user);
            return false;
        }
        if (!keyComp.IsForged)
        {
            denyReason = Loc.GetString("lockkey-not-forged");
            //_popup.PopupClient(denyReason, target, user);
            return false;
        }
        if (keyComp.Key != target.Comp.AllowedKey)
        {
            denyReason = Loc.GetString("lockkey-reader-fail");
            //_popup.PopupClient(denyReason, target, user);
            return false;
        }
        return true;
    }
}
