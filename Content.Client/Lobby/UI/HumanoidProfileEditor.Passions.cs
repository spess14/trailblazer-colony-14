using Content.Client._tc14.UI;
using Content.Shared._tc14.Skills.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private SkillPicker? _skillPicker;

    private void RefreshSkills()
    {
        if (Profile is null || _skillPicker is not null)
            return;
        _skillPicker = new SkillPicker(Profile.Passions);
        TabContainer.AddChild(_skillPicker);
        TabContainer.SetTabTitle(TabContainer.ChildCount - 1, Loc.GetString("skills-passionmenu-name"));

        _skillPicker.OnPassionsChanged += OnPassionsChange;
    }

    private void OnPassionsChange(Dictionary<ProtoId<SkillPrototype>, int> content)
    {
        if (Profile is null)
            return;

        Profile = Profile.WithSkills(content);
        SetDirty();
    }

    private void UpdateSkills()
    {
        if (_skillPicker is null || Profile is null)
            return;
        _skillPicker.RebuildEntries(Profile.Passions);
    }
}
