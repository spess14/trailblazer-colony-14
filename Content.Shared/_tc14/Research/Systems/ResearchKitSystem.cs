using System.Linq;
using Content.Shared._tc14.Research.Components;
using Content.Shared._tc14.Research.Prototypes;
using Content.Shared._tc14.Skills.Systems;
using Content.Shared.EntityConditions;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Stacks;
using Content.Shared.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared._tc14.Research.Systems;

/// <summary>
/// Handles ResearchKitComponent, also handles game logic for the UI.
/// </summary>
public sealed partial class ResearchKitSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;
    [Dependency] private SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private SharedEntityConditionsSystem _conditions = default!;
    [Dependency] private SharedStackSystem _stack = default!;
    [Dependency] private PlayerSkillsSystem _skills = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ResearchKitComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ResearchKitComponent, BeforeActivatableUIOpenEvent>(OnBeforeUiOpened);
        SubscribeLocalEvent<ResearchKitComponent, AfterInteractEvent>(OnBountyCheck);
        SubscribeLocalEvent<ResearchKitComponent, AfterAutoHandleStateEvent>(OnKitState);
    }

    private void OnKitState(Entity<ResearchKitComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateUi(ent);
    }

    private void OnBountyCheck(Entity<ResearchKitComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target == null || !args.CanReach || HasComp<ResearchTableComponent>(args.Target))
            return;
        var target = args.Target.Value;
        var bountyMatched = false;
        ResearchKitBounty? matchedBounty = null;
        foreach (var bounty in ent.Comp.Bounties)
        {
            var proto = _protoMan.Index(bounty.BountyPrototype);
            if (!_conditions.TryConditions(target, proto.Conditions))
                continue;
            bountyMatched = true;
            var amount = 1;
            if (TryComp<StackComponent>(target, out var stack)) // TODO implement solution mode
            {
                amount = Math.Min(proto.MaxAmount - bounty.Progress, _stack.GetCount((target, stack)));
                _stack.ReduceCount((target, stack), amount);
            }
            else
            {
                _entMan.PredictedQueueDeleteEntity(target); // TODO in solution mode, remove the solution
            }
            if (_timing.IsFirstTimePredicted)
                bounty.Progress += amount;
            if (bounty.Progress >= proto.MaxAmount)
            {
                _popup.PopupPredicted(Loc.GetString("researchbounty-done"), target, args.User);
                matchedBounty = bounty;
            }
            else
            {
                _popup.PopupPredicted(Loc.GetString("researchbounty-partial",
                        ("progress", bounty.Progress),
                        ("total", proto.MaxAmount)),
                    target,
                    args.User);
            }
            break;
        }
        if (!bountyMatched)
            _popup.PopupPredicted(Loc.GetString("researchbounty-nothing-found"), target, args.User);
        if (matchedBounty != null && _entMan.TryGetComponent<TCResearchPointSourceComponent>(ent.Owner, out var comp))
        {
            var proto = _protoMan.Index(matchedBounty.BountyPrototype);
            var multiplier = GetMultiplier(ent, args.User);
            var pointTotal = 0;
            foreach (var pair in proto.RewardedPoints)
            {
                if (comp.StoredPoints.ContainsKey(pair.Key))
                {
                    comp.StoredPoints[pair.Key] += (int)(pair.Value * multiplier);
                }
                else
                {
                    comp.StoredPoints.Add(pair.Key, (int)(pair.Value * multiplier));
                }

                pointTotal += (int)(pair.Value * multiplier);
            }

            var ev = new PlayerSkillActionEvent("SkillResearch", pointTotal * 0.01);
            RaiseLocalEvent(args.User, ref ev);

            ent.Comp.Bounties.Remove(matchedBounty);
            GenerateBounties(ent);
            Dirty(ent.Owner, comp);
        }
        Dirty(ent);
        args.Handled = true;
    }

    private void OnMapInit(Entity<ResearchKitComponent> ent, ref MapInitEvent args)
    {
        GenerateBounties(ent);
    }

    private FixedPoint2 GetMultiplier(Entity<ResearchKitComponent> ent, EntityUid player)
    {
        return (1 + _skills.GetSkillLevel("SkillResearch", player) * 0.05) * ent.Comp.PointsMultiplier;
    }

    private void GenerateBounties(Entity<ResearchKitComponent> ent)
    {
        var bounties = _protoMan.EnumeratePrototypes<ResearchBountyPrototype>().ToList();
        var random = SharedRandomExtensions.PredictedRandom(_timing, GetNetEntity(ent));
        while (ent.Comp.Bounties.Count < ent.Comp.MaxBounties)
        {
            var bounty = random.Pick(bounties);
            ent.Comp.Bounties.Add(new ResearchKitBounty{BountyPrototype = bounty.ID});
            random.SetSeed(random.Next()); // change the seed in a predictable way, otherwise we get 6 same bounties
        }
        Dirty(ent);
    }

    private void OnBeforeUiOpened(Entity<ResearchKitComponent> ent, ref BeforeActivatableUIOpenEvent args)
    {
        UpdateUi(ent);
    }

    private void UpdateUi(Entity<ResearchKitComponent> ent)
    {
        if (_uiSystem.TryGetOpenUi(ent.Owner, ResearchKitUiKey.Key, out var bui))
            bui.Update();
    }
}

[Serializable, NetSerializable]
public enum ResearchKitUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class ResearchKitState(List<ResearchKitBounty> bounties) : BoundUserInterfaceState
{
    public List<ResearchKitBounty> Bounties = bounties;
}
