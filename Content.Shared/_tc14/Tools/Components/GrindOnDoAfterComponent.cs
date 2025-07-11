using Content.Shared.DoAfter;
using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Tools.Components;

/// <summary>
/// Items with this component will grind everything in ItemSlots into a solution
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class GrindOnDoAfterComponent : Component
{
    [DataField]
    public DoAfterId? DoAfter;

    /// <summary>
    /// Do after time to grind the items.
    /// </summary>
    [DataField]
    public TimeSpan GrindTime = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Where to add the solution.
    /// </summary>
    [DataField]
    public string Solution = "default";

    /// <summary>
    /// Where to take grindable ingredients from.
    /// </summary>
    [DataField]
    public string ItemSlot = "default";
}
