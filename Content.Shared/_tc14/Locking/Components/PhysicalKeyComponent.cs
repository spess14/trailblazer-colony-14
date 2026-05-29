using Robust.Shared.GameStates;

namespace Content.Shared._tc14.Locking.Components;

/// <summary>
/// Used for physical keys, the kind you insert into a lock and rotate them and stuff.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PhysicalKeyComponent : Component
{
    /// <summary>
    /// A 16-bit key used as a value to compare to. It is small by design (so that very rare collisions can happen).
    /// </summary>
    [DataField, AutoNetworkedField]
    public ushort Key;

    /// <summary>
    /// The key can only be used when this is true.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsForged;
}
