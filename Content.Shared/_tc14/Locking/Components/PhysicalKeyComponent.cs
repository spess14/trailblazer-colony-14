using Content.Shared._tc14.Locking.Systems;
using JetBrains.Annotations;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Locking.Components;

/// <summary>
/// Used for physical keys, the kind you insert into a lock and rotate them and stuff.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(KeyForgingSystem))]
public sealed partial class PhysicalKeyComponent : Component
{
    /// <summary>
    /// An 8-bit key used as a value to compare to. It is small by design (so that rare collisions can happen).
    /// </summary>
    [DataField, AutoNetworkedField]
    public ushort Key;

    /// <summary>
    /// The key can only be used when this is true.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsForged;
}

[Serializable, NetSerializable]
public enum PhysicalKeyVisuals : ushort
{
    [UsedImplicitly] A,
    [UsedImplicitly] B,
    [UsedImplicitly] C,
    [UsedImplicitly] D,
    [UsedImplicitly] E,
    [UsedImplicitly] F,
    [UsedImplicitly] G,
    [UsedImplicitly] H,
    [UsedImplicitly] I,
    [UsedImplicitly] J,
    [UsedImplicitly] K,
    [UsedImplicitly] L,
    [UsedImplicitly] M,
    [UsedImplicitly] N,
    [UsedImplicitly] O,
    [UsedImplicitly] P,
}
