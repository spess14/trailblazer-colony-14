using Content.Shared._tc14.Locking.Components;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;

namespace Content.Shared._tc14.Locking.Systems;

/// <summary>
/// Handles adding locks to doors, lockers, etc.
/// </summary>
public sealed partial class PhysicalLockTargetSystem : EntitySystem
{
    [Dependency] private LockSystem _lock = default!;
    [Dependency] private SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PhysicalLockTargetComponent, InteractUsingEvent>(OnAddingLock);
    }

    private void OnAddingLock(Entity<PhysicalLockTargetComponent> ent, ref InteractUsingEvent args)
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
        _lock.Unlock(ent, null, lockComp);
        _popup.PopupClient(Loc.GetString("lockkey-locking-success"), args.User);
        args.Handled = true;
        PredictedQueueDel(args.Used);
    }
}
