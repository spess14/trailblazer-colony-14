using Content.Shared.Trigger.Components.Triggers;
using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Trigger.Components.Conditions;

/// <summary>
/// Triggers when the entity is damaged.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnDamageComponent : BaseTriggerOnXComponent
{
}
