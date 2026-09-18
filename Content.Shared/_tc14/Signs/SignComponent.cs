using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._tc14.Signs;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SignComponent : Component
{
    /// <summary>
    /// Max length of the text on the sign.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int MaxSymbols = 100;

    /// <summary>
    /// Sign contents.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string Text = string.Empty;

    /// <summary>
    /// LocId that will be the "edit text on the sign" verb name.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId VerbName = "sign-verb-write";

    /// <summary>
    /// Icon for the verb mentioned above.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SpriteSpecifier? VerbImage = new SpriteSpecifier.Texture(new("/Textures/Interface/pencil.png"));

    /// <summary>
    /// Title of the edit dialog.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId DialogTitle = "sign-dialog-title";

    /// <summary>
    /// Prompt of the edit dialog.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId DialogPrompt = "sign-dialog-prompt";

    /// <summary>
    /// LocId that is used for the popup when the text is changed.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId PopupLoc = "sign-popup";

    /// <summary>
    /// Audio that plays when changing text.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SoundSpecifier? ChangedTextSound = new SoundCollectionSpecifier("PaperScribbles", AudioParams.Default.WithVariation(0.1f));
}
