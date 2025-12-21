using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Client.Stylesheets.SheetletConfigs;
using Content.Client.Stylesheets.Sheetlets;
using Content.Client.Stylesheets.Stylesheets;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Content.Client.Stylesheets.StylesheetHelpers;

namespace Content.Client._tc14.Research.UI;

[CommonSheetlet]
public sealed class ResearchTableSheetlet : Sheetlet<NanotrasenStylesheet>
{
    public override StyleRule[] GetRules(NanotrasenStylesheet sheet, object config)
    {
        IWindowConfig windowCfg = sheet;

        var buttonBox = ResCache.GetTexture("/Textures/Interface/Nano/rounded_button_bordered.svg.96dpi.png")
            .IntoPatch(StyleBox.Margin.All, 4);
        return
        [
            E<Button>().Identifier("ResearchItemButton").Box(buttonBox),
            // The color of the border is defined in ResearchTableItem.xaml.cs depending on discipline
            E<PanelContainer>().Identifier("ResearchItemBorder").Box(buttonBox),
            E<PanelContainer>().Identifier("ResearchItemBorder").Prop(PanelContainer.StylePropertyPanel, StyleBoxHelpers.SquareStyleBox(sheet)),
        ];

    }
}
