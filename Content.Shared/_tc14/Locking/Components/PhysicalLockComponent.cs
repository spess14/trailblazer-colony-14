using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Locking.Components;

/// <summary>
/// Used for the lock *item* that is supposed to read from a <see cref="PhysicalKeyComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PhysicalLockComponent : Component
{
    /// <summary>
    /// Which key is this lock supposed to accept?
    /// </summary>
    [DataField, AutoNetworkedField]
    public ushort AllowedKey;

    /// <summary>
    /// The lock can only be used when this is true.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsForged;
}
