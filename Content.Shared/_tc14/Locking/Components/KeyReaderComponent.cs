using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Locking.Components;

/// <summary>
/// For entities that have a lock that should be unlocked with a <see cref="PhysicalKeyComponent"/>. Not added to the lock itself - only to lockables.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class KeyReaderComponent : Component
{
    /// <summary>
    /// Which key does this lock accept?
    /// </summary>
    [DataField, AutoNetworkedField]
    public ushort AllowedKey;
}
