using Content.Shared._tc14.Locking.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Random;

namespace Content.Shared._tc14.Locking.Systems;

/// <summary>
/// Handles setting the IsForged flag for both PhysicalKey and PhysicalLock.
/// </summary>
public sealed partial class KeyForgingSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private INetManager _net = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PhysicalLockComponent, InteractUsingEvent>(OnInteractedWithLock);
        SubscribeLocalEvent<PhysicalKeyComponent, InteractUsingEvent>(OnInteractedWithKey);
        SubscribeLocalEvent<PhysicalKeyComponent, ExaminedEvent>(KeyOnExamine);
        SubscribeLocalEvent<PhysicalLockComponent, ExaminedEvent>(LockOnExamine);
    }

    private void LockOnExamine(Entity<PhysicalLockComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString(ent.Comp.IsForged
            ? "lockkey-examine-lock-forged"
            : "lockkey-examine-lock-unforged"));
    }

    private void KeyOnExamine(Entity<PhysicalKeyComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString(ent.Comp.IsForged
            ? "lockkey-examine-key-forged"
            : "lockkey-examine-key-unforged"));
    }

    [PublicAPI]
    public void ForgeKey(Entity<PhysicalKeyComponent> ent, ushort key)
    {
        ent.Comp.IsForged = true;
        ent.Comp.Key = key;
        if (_net.IsClient)
            return;
        for (var i = 0; i<=15; i++)
        {
            var ret = (key & (2 ^ i)) != 0;
            _appearance.SetData(ent, (PhysicalKeyVisuals)i, ret);
        }
    }

    [PublicAPI]
    public void ForgeLock(Entity<PhysicalLockComponent> ent, ushort key)
    {
        ent.Comp.IsForged = true;
        ent.Comp.AllowedKey = key;
    }

    // TODO refactor PhysicalKey and PhysicalLock into one component
    private void OnInteractedWithLock(Entity<PhysicalLockComponent> ent, ref InteractUsingEvent args)
    {
        var user = args.User;
        var used = args.Used;

        if (!TryComp<PhysicalKeyComponent>(used, out var keyComp))
        {
            if (!TryComp<PhysicalLockComponent>(used, out var usedLockComp))
                return;
            LockOnLock(ent, (used, usedLockComp), user);
        }
        else
        {
            KeyOnLock(ent, (used, keyComp), user);
        }
    }

    private void OnInteractedWithKey(Entity<PhysicalKeyComponent> ent, ref InteractUsingEvent args)
    {
        var user = args.User;
        var used = args.Used;

        if (!TryComp<PhysicalKeyComponent>(used, out var keyComp))
        {
            if (!TryComp<PhysicalLockComponent>(used, out var usedLockComp))
                return;
            KeyOnLock((used, usedLockComp), ent, user);
        }
        else
        {
            KeyOnKey(ent, (used, keyComp), user);
        }
    }

    private void KeyOnKey(Entity<PhysicalKeyComponent> keyEnt, Entity<PhysicalKeyComponent> usedKeyEnt, EntityUid user)
    {
        if (usedKeyEnt.Comp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-keyforged"), user);
            return;
        }

        if (!keyEnt.Comp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-keyunforged"), user);
            return;
        }
        ForgeKey(usedKeyEnt, keyEnt.Comp.Key);
        _popup.PopupClient(Loc.GetString("lockkey-forging-key-copy-success"), user);
        Dirty(usedKeyEnt);
        _audio.PlayPredicted(new SoundPathSpecifier("/Audio/_tc14/Items/chisel_use.ogg"), usedKeyEnt.Owner, user);
    }

    private void KeyOnLock(Entity<PhysicalLockComponent> lockEnt, Entity<PhysicalKeyComponent> keyEnt, EntityUid user)
    {
        if (keyEnt.Comp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-keyforged"), user);
            return;
        }

        if (lockEnt.Comp.IsForged)
        {
            ForgeKey(keyEnt, lockEnt.Comp.AllowedKey);
            _popup.PopupClient(Loc.GetString("lockkey-forging-key-success"), user);
        }
        else
        {
            var key = (ushort)_random.Next(ushort.MinValue, ushort.MaxValue + 1);
            ForgeKey(keyEnt, key);
            ForgeLock(lockEnt, key);
            Dirty(lockEnt);
            _popup.PopupClient(Loc.GetString("lockkey-forging-success"), user);
        }
        Dirty(keyEnt);
        _audio.PlayPredicted(new SoundPathSpecifier("/Audio/_tc14/Items/chisel_use.ogg"), keyEnt.Owner, user);
    }

    private void LockOnLock(Entity<PhysicalLockComponent> lockEnt, Entity<PhysicalLockComponent> usedLockEnt, EntityUid user)
    {
        if (usedLockEnt.Comp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-lockforged"), user);
            return;
        }

        if (!lockEnt.Comp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-lockunforged"), user);
            return;
        }
        ForgeLock(usedLockEnt, lockEnt.Comp.AllowedKey);
        _popup.PopupClient(Loc.GetString("lockkey-forging-lock-copy-success"), user);
        Dirty(usedLockEnt);
        _audio.PlayPredicted(new SoundPathSpecifier("/Audio/_tc14/Items/chisel_use.ogg"), usedLockEnt.Owner, user);
    }
}
