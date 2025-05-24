using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Input;
using Robust.Shared.Utility;
<<<<<<< HEAD
using Content.Client.UserInterface.ControlExtensions;
=======
>>>>>>> 494861dc3d938b752567bead5bf40132fba03107

namespace Content.Client.Guidebook.RichText;

[UsedImplicitly]
public sealed class TextLinkTag : IMarkupTag
{
<<<<<<< HEAD
    public static Color LinkColor => Color.CornflowerBlue;

=======
>>>>>>> 494861dc3d938b752567bead5bf40132fba03107
    public string Name => "textlink";

    public Control? Control;

    /// <inheritdoc/>
    public bool TryGetControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
    {
        if (!node.Value.TryGetString(out var text)
            || !node.Attributes.TryGetValue("link", out var linkParameter)
            || !linkParameter.TryGetString(out var link))
        {
            control = null;
            return false;
        }

        var label = new Label();
        label.Text = text;

        label.MouseFilter = Control.MouseFilterMode.Stop;
<<<<<<< HEAD
        label.FontColorOverride = LinkColor;
=======
        label.FontColorOverride = Color.CornflowerBlue;
>>>>>>> 494861dc3d938b752567bead5bf40132fba03107
        label.DefaultCursorShape = Control.CursorShape.Hand;

        label.OnMouseEntered += _ => label.FontColorOverride = Color.LightSkyBlue;
        label.OnMouseExited += _ => label.FontColorOverride = Color.CornflowerBlue;
        label.OnKeyBindDown += args => OnKeybindDown(args, link);

        control = label;
        Control = label;
        return true;
    }

    private void OnKeybindDown(GUIBoundKeyEventArgs args, string link)
    {
        if (args.Function != EngineKeyFunctions.UIClick)
            return;

        if (Control == null)
            return;

<<<<<<< HEAD
        if (Control.TryGetParentHandler<ILinkClickHandler>(out var handler))
            handler.HandleClick(link);
        else
            Logger.Warning("Warning! No valid ILinkClickHandler found.");
=======
        var current = Control;
        while (current != null)
        {
            current = current.Parent;

            if (current is not ILinkClickHandler handler)
                continue;
            handler.HandleClick(link);
            return;
        }
        Logger.Warning($"Warning! No valid ILinkClickHandler found.");
>>>>>>> 494861dc3d938b752567bead5bf40132fba03107
    }
}

public interface ILinkClickHandler
{
    public void HandleClick(string link);
}
