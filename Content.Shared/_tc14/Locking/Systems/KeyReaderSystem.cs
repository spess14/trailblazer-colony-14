using System.Diagnostics.CodeAnalysis;
using Content.Shared._tc14.Locking.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Lock;
using Content.Shared.Prying.Components;
using Content.Shared.Verbs;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._tc14.Locking.Systems;

/// <summary>
/// Handles reading from a physical key.
/// </summary>
public sealed partial class KeyReaderSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private KeyForgingSystem _keyforge = default!;

    private const string LockPrototype = "Lock";

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KeyReaderComponent, FindAvailableLocksEvent>(OnFindAvailableLocks);
        SubscribeLocalEvent<KeyReaderComponent, CheckUserHasLockAccessEvent>(OnCheckLockAccess);
        SubscribeLocalEvent<KeyReaderComponent, BeforePryEvent>(OnBeforePry, after: [typeof(LockSystem)]);
        SubscribeLocalEvent<KeyReaderComponent, GetVerbsEvent<AlternativeVerb>>(AddRemoveLockVerb);
    }

    private void AddRemoveLockVerb(Entity<KeyReaderComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanComplexInteract || !args.CanAccess)
            return;

        var lockComp = CompOrNull<LockComponent>(ent);
        var locked = lockComp?.Locked == true;

        AlternativeVerb verb = new()
        {
            Disabled = locked,
            Act = () =>
            {
                if (lockComp is null)
                    return;
                var key = ent.Comp.AllowedKey;
                var lockUid = PredictedSpawnAtPosition(LockPrototype, Transform(ent).Coordinates);
                var physLockComp = Comp<PhysicalLockComponent>(lockUid);
                _keyforge.ForgeLock((lockUid, physLockComp), key);
                Dirty(lockUid, physLockComp);
                var lockEv = new FindAvailableLocksEvent(ent);
                RaiseLocalEvent(ent, ref lockEv);
                if (lockEv.FoundReaders == LockTypes.Key)
                    RemCompDeferred<LockComponent>(ent);
                RemCompDeferred<KeyReaderComponent>(ent);
            },
            Text = Loc.GetString("lockkey-remove-verb-text"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/screwdriver.png")),
        };
        args.Verbs.Add(verb);
    }

    private void OnBeforePry(Entity<KeyReaderComponent> ent, ref BeforePryEvent args)
    {
        if (args.Cancelled)
            return;

        if (CompOrNull<LockComponent>(ent)?.Locked != true)
            return;

        args.Message = ent.Comp.PryFailedPopup;

        args.Cancelled = true;
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
