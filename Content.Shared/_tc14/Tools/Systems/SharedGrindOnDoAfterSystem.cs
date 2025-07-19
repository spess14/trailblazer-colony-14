using Content.Shared._tc14.Tools.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Kitchen.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Tools.Systems;

/// <summary>
/// Handles GrindOnDoAfterComponent
/// </summary>
public sealed class SharedGrindOnDoAfterSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popups = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GrindOnDoAfterComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
    }

    private void OnGetVerbs(EntityUid uid, GrindOnDoAfterComponent component, ref GetVerbsEvent<InteractionVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || args.Hands == null)
            return;

        var user = args.User;

        args.Verbs.Add(new InteractionVerb()
        {
            Text = Loc.GetString("grind-verb"),
            Act = () => Grind(uid, component, user),
        });
    }

    private void Grind(EntityUid uid, GrindOnDoAfterComponent component, EntityUid user)
    {
        var item = _itemSlots.GetItemOrNull(uid, component.ItemSlot);

        if (!HasComp<ExtractableComponent>(item))
        {
            _popups.PopupClient(Loc.GetString("grind-cannot-grind"), user);
            return;
        }

        var doAfterArgs = new DoAfterArgs(EntityManager, user, component.GrindTime, new GrindDoAfterEvent(), uid, uid)
        {
            BreakOnDamage = true,
            NeedHand = true,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
    }
}

[Serializable, NetSerializable]
public sealed partial class GrindDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone() => this;
}
