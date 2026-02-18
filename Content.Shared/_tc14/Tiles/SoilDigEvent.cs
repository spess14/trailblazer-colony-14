using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._tc14.Tiles;

[Serializable, NetSerializable]
public sealed partial class SoilDigEvent : SimpleDoAfterEvent
{
    [DataField]
    public EntProtoId SoilPrototypeName;
}
