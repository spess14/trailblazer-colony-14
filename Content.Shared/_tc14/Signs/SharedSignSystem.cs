using Content.Shared.Examine;

namespace Content.Shared._tc14.Signs;

/// <summary>
/// Handles displaying text in the examine window.
/// </summary>
public sealed class SharedSignSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SignComponent, ExaminedEvent>(OnExamine);
    }

    private void OnExamine(Entity<SignComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.Text == string.Empty)
            return;
        using (args.PushGroup(nameof(SignComponent)))
        {
            args.PushMarkup(Loc.GetString("sign-text", ("text", ent.Comp.Text)));
        }
    }
}
