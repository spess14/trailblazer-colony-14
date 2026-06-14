using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Locking.Components;

/// <summary>
/// Marker component - marks the entities that can be locked using physical locks.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PhysicalLockTargetComponent : Component;
