namespace Content.Shared._tc14.Chemistry;

[ByRefEvent]
public sealed partial class HeatEntityEvent : EntityEventArgs
{
    [DataField]
    public EntityUid EntityUid;

    [DataField]
    public float Energy;
}
