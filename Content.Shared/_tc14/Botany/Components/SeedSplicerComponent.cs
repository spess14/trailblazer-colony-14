using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._tc14.Botany.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SeedSplicerComponent : Component
{

}

[Serializable, NetSerializable]
public enum SeedSplicerUiKey : byte
{
    Key,
}
