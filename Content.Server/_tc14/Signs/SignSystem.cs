using Content.Server.Administration;
using Content.Shared._tc14.Signs;
using Content.Shared.Database;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server._tc14.Signs;

/// <summary>
/// Handles writing on signs. Has to be on server due to QuickDialogSystem.
/// </summary>
public sealed partial class SignSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private QuickDialogSystem _quickDialog = default!;
    [Dependency] private SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SignComponent, GetVerbsEvent<ActivationVerb>>(AddVerb);
    }

    private void AddVerb(Entity<SignComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
    {
        var user = args.User;

        if (!TryComp(user, out ActorComponent? actor) || !args.CanInteract)
            return;

        var writeVerb = new ActivationVerb
        {
            Text = Loc.GetString(ent.Comp.VerbName),
            Icon = ent.Comp.VerbImage,
            Act = () =>
            {
                _quickDialog.OpenDialog(actor.PlayerSession,
                    Loc.GetString(ent.Comp.DialogTitle),
                    Loc.GetString(ent.Comp.DialogPrompt),
                    (string message) =>
                    {
                        ent.Comp.Text = FormattedMessage.EscapeText(message);
                        Dirty(ent);
                        _audio.PlayPvs(ent.Comp.ChangedTextSound, ent);
                        _popup.PopupEntity(Loc.GetString("sign-popup", ("user", Identity.Entity(user, EntityManager)), ("text", FormattedMessage.EscapeText(message))), ent);
                    });
            },
            Impact = LogImpact.Low,
        };
        args.Verbs.Add(writeVerb);
    }
}
