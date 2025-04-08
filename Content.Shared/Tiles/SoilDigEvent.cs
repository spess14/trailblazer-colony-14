using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Tiles;

[Serializable, NetSerializable]
public sealed partial class SoilDigEvent : SimpleDoAfterEvent
{
    [DataField("soilDrop", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SoilPrototypeName;
}
