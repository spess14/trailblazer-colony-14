using Content.Shared._tc14.Locking.Components;
using Content.Shared.Doors.Components;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared._tc14.Locking.Systems;

/// <summary>
/// Handles setting the IsForged flag for both PhysicalKey and PhysicalLock.
/// </summary>
public sealed partial class KeyForgingSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private LockSystem _lock = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PhysicalLockComponent, InteractUsingEvent>(OnInteractedWithItem);
        SubscribeLocalEvent<DoorComponent, InteractUsingOnDoorEvent>(OnAddingLock);
    }

    private void OnAddingLock(Entity<DoorComponent> ent, ref InteractUsingOnDoorEvent args)
    {
        if (!TryComp<PhysicalLockComponent>(args.Used, out var physLockComp) || HasComp<KeyReaderComponent>(ent))
            return;

        if (!physLockComp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-locking-unforged"), args.User);
            return;
        }

        var lockComp = EnsureComp<LockComponent>(ent);
        var physReader = EnsureComp<KeyReaderComponent>(ent);
        physReader.AllowedKey = physLockComp.AllowedKey;
        _lock.Unlock(ent, args.User, lockComp);
        _popup.PopupClient(Loc.GetString("lockkey-locking-success"), args.User);
        args.Handled = true;
        PredictedQueueDel(args.Used);
    }

    private void OnInteractedWithItem(Entity<PhysicalLockComponent> ent, ref InteractUsingEvent args)
    {
        var user = args.User;
        var used = args.Used;

        if (!TryComp<PhysicalKeyComponent>(used, out var keyComp) || !TryComp<ActorComponent>(user, out var actorComp))
            return;

        if (ent.Comp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-lockforged"), user);
            return;
        }

        if (keyComp.IsForged)
        {
            _popup.PopupClient(Loc.GetString("lockkey-forging-keyforged"), user);
            return;
        }

        var key = (ushort)_random.Next(ushort.MinValue, ushort.MaxValue + 1);
        ent.Comp.AllowedKey = key;
        keyComp.Key = key;
        ent.Comp.IsForged = true;
        keyComp.IsForged = true;
        Dirty(ent);
        Dirty(used, keyComp);
        _popup.PopupClient(Loc.GetString("lockkey-forging-success"), user);
        _audio.PlayPredicted(new SoundPathSpecifier("/Audio/_tc14/Items/chisel_use.ogg"), used, user);
    }
}

/// <summary>
/// This is an event to work around the duplicate subscription with the PryingSystem.
/// </summary>
[PublicAPI]
public sealed class InteractUsingOnDoorEvent : HandledEntityEventArgs
{
    /// <summary>
    ///     Entity that triggered the interaction.
    /// </summary>
    public EntityUid User { get; }

    /// <summary>
    ///     Entity that the user used to interact.
    /// </summary>
    public EntityUid Used { get; }

    /// <summary>
    ///     Entity that was interacted on.
    /// </summary>
    public EntityUid Target { get; }

    /// <summary>
    ///     The original location that was clicked by the user.
    /// </summary>
    public EntityCoordinates ClickLocation { get; }

    public InteractUsingOnDoorEvent(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation)
    {
        // Interact using should not have the same used and target.
        // That should be a use-in-hand event instead.
        // If this is not the case, can lead to bugs (e.g., attempting to merge a item stack into itself).
        DebugTools.Assert(used != target);

        User = user;
        Used = used;
        Target = target;
        ClickLocation = clickLocation;
    }
}
