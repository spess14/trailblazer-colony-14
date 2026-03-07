using Content.Shared.Inventory;

namespace Content.Shared._tc14.Damage.Components;

/// <summary>
/// Similar to DamageOnInteractProtectionComponent, adding this to clothing protects the user from DamageOnHoldingComponent.
/// </summary>
[RegisterComponent]
public sealed partial class DamageOnHoldingProtectionComponent : Component, IClothingSlots
{
    /// <summary>
    /// Only protects if the item is in the correct slot
    /// i.e. having gloves in your pocket doesn't protect you, it has to be on your hands
    /// </summary>
    [DataField]
    public SlotFlags Slots { get; set; } = SlotFlags.GLOVES;
}
