using Content.Shared._Starlight.CollectiveMind; // Starlight - Collective Mind
using Content.Shared._tc14.Skills.Prototypes;
using Content.Shared.CharacterInfo;
using Content.Shared.FixedPoint;
using Content.Shared.Objectives;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client.CharacterInfo;

public sealed class CharacterInfoSystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _players = default!;

    public event Action<CharacterData>? OnCharacterUpdate;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<CharacterInfoEvent>(OnCharacterInfoEvent);
    }

    public void RequestCharacterInfo()
    {
        var entity = _players.LocalEntity;
        if (entity == null)
        {
            return;
        }

        RaiseNetworkEvent(new RequestCharacterInfoEvent(GetNetEntity(entity.Value)));
    }

    // TC14: added skills info
    private void OnCharacterInfoEvent(CharacterInfoEvent msg, EntitySessionEventArgs args)
    {
        var entity = GetEntity(msg.NetEntity);
        var data = new CharacterData(entity, msg.JobTitle, msg.Objectives, msg.CollectiveMinds, msg.Briefing, Name(entity), msg.Skills); // Starlight - Collective Mind - Add data entry for collective minds.

        OnCharacterUpdate?.Invoke(data);
    }

    public List<Control> GetCharacterInfoControls(EntityUid uid)
    {
        var ev = new GetCharacterInfoControlsEvent(uid);
        RaiseLocalEvent(uid, ref ev, true);
        return ev.Controls;
    }

    // TC14: added skills info
    public readonly record struct CharacterData(
        EntityUid Entity,
        string Job,
        Dictionary<string, List<ObjectiveInfo>> Objectives,
        Dictionary<CollectiveMindPrototype, CollectiveMindMemberData>? CollectiveMinds, // Starlight - Collective Mind - Collective mind data entry.
        string? Briefing,
        string EntityName,
        Dictionary<ProtoId<SkillPrototype>, FixedPoint2> Skills
    );

    /// <summary>
    /// Event raised to get additional controls to display in the character info menu.
    /// </summary>
    [ByRefEvent]
    public readonly record struct GetCharacterInfoControlsEvent(EntityUid Entity)
    {
        public readonly List<Control> Controls = new();

        public readonly EntityUid Entity = Entity;
    }
}
