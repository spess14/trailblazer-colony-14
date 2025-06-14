using Content.Shared.Construction.Components;
using Content.Shared.Lathe.Prototypes;
using Content.Shared.Research.Prototypes;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Blueprint;

[Serializable, NetSerializable, Prototype]
public sealed class BluebenchResearchPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    ///     Name displayed in the blueprint workbench.
    /// </summary>
    [DataField]
    public string? Name;

    /// <summary>
    ///     Describes some research stuff
    /// </summary>
    [DataField]
    public string? Description;

    /// <summary>
    ///     An entity whose sprite is displayed in the ui in place of the actual recipe result.
    /// </summary>
    [DataField]
    public SpriteSpecifier? Icon;

    /// <summary>
    /// The stacks needed to research this blueprint
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<StackPrototype>, int> StackRequirements = new();

    /// <summary>
    /// Entities needed to research this blueprint, discriminated by tag
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<TagPrototype>, GenericPartInfo> TagRequirements = new();

    /// <summary>
    /// Entities needed to research this blueprint, discriminated by component.
    /// </summary>
    [DataField]
    public Dictionary<string, GenericPartInfo> ComponentRequirements = new();

    /// <summary>
    /// The recipes produced with the blueprint
    /// </summary>
    [DataField]
    public HashSet<ProtoId<LatheRecipePrototype>> OutputRecipes = [];

    /// <summary>
    /// The recipe packs whose recipes will be produced with the blueprint
    /// </summary>
    [DataField]
    public HashSet<ProtoId<LatheRecipePackPrototype>>? OutputPacks = [];

    /// <summary>
    /// Tags that are attached to the blueprint.
    /// By default, the "BlueprintAutolathe" tag is attached.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<TagPrototype>> OutputTags = ["BlueprintAutolathe"];

    /// <summary>
    /// Specifies the research project that needs to be completed before this one can start
    /// </summary>
    [DataField]
    public HashSet<ProtoId<BluebenchResearchPrototype>>? RequiredResearch;
}
