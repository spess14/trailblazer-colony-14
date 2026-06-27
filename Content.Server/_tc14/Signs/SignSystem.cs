using Content.Server.Administration;
using Content.Shared._tc14.Signs;
using Content.Shared.Database;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server._tc14.Signs;

/// <summary>
/// Handles writing on signs. Has to be on server due to QuickDialogSystem.
/// </summary>
public sealed class SignSystem : EntitySystem
{
    [Dependency] private readonly QuickDialogSystem _quickDialog = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SignComponent, GetVerbsEvent<ActivationVerb>>(AddVerb);
    }

    private void AddVerb(Entity<SignComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
    {
        if (!TryComp(args.User, out ActorComponent? actor) || !args.CanInteract)
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
                    });
            },
            Impact = LogImpact.Low,
        };
        args.Verbs.Add(writeVerb);
    }
}
