# Trailblazer Colony 14 Contributing Guidelines
Thanks for contributing to TC14! In order to avoid any nasty merge conflicts or confusion when working on the project, please follow these guidelines.

Our guidelines are practically the same as [Moffstation's Contribution Guidelines](https://github.com/moff-station/moff-station-14/blob/master/CONTRIBUTING.md), with some minor tweaks to fit our project.

As a base, we expect you to follow the [Space Station 14 Contribution Guidelines](https://docs.spacestation14.com/en/general-development/codebase-info/pull-request-guidelines.html) as well.

> [!WARNING]
> Do not make any changes in the GitHub web editor (webedits), as we want you to test your changes in-game before submitting a pull request. This holds true even for small YAML balance tweaks.

> [!TIP]
> It's highly recommended to set up a development environment when working on the project. A general step-by-step guide is available in the upstream docs [here](https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html).
>
>We highly recommend you use an IDE like [Jetbrains Rider](https://www.jetbrains.com/rider/). It's free for non-commercial use, and it basically holds your hand.

## AI-generated content
We do not accept any low-effort or wholesale AI-generated contributions.
This includes the following, but is not limited to:
- Any code (C#, YAML, XML, etc.) generated from tools like ChatGPT, Github Copilot, Cursor, and whatever ChatGPT wrapper that's currently the hottest thing on the block.
- Any artwork, sound files, or other assets.
- Auto-generated documentation, GitHub's issue/PR changes summarization tools, among other tools.

Exceptions to this are simple tools, for example:
- Machine learning-assisted full line code completion.
- Intellisense/ReSharper machine learning-sorted autocompletion suggestions (or any other ML-assisted sorting operation).
- Machine learning-assisted grammar error correction.

Maintainers still hold the right to deny contributions that have been created by AI, even if they only appear as such.

## TC14-exclusive content
Space Station 14 allows separate content to be added to the game that is not part of the upstream project in a clean and easy way. Separate content is placed in a new subfolder namespace, `_tc14`. This is to avoid conflicts with upstream content.

This includes new prototypes, new sprites, new sounds, new maps, and so on. If you are adding new content to the game that is different from upstream, it should be placed in the `_tc14` namespace.

Examples:

- `Content.Server/_tc14/Skills/Commands/ViewSkillsCommand.cs`
- `Resources/Prototypes/_tc14/game_presets.yml`
- `Resources/Audio/_tc14/Rimworld/Blue.ogg`
- `Resources/Textures/_tc14/Interface/check.png`
- `Resources/Locale/en-US/_tc14/research/disciplines.ftl`

Try to mirror the original file structure of the game as much as possible when adding new content in our custom namespace. This makes it easier for others to find and understand your changes.

## Changes to upstream files
If you make changes to files that are part of the upstream project, you must comment them respectively. This is to make it easier for us to keep track of what changes we have made to the upstream project (for merge conflict resolution)

Overall, the comment should state that the code modified is a TC14 modification, and the reason why.

If you delete any content from upstream, do not delete the content! Instead, comment it out, and comment why you did so above the commented out section.

If you changed any content from upstream (for example, for balance or for disabling unwanted behavior), comment why you did so on the same line as the change.

Fluent (.ftl) files, commonly used for localization, do not support same-line comments. Always comment above the line you are changing.

If you're adding a lot of C# code to upstream files, you should put it in a [partial class](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods) in a new file in the `_tc14` namespace. This makes it easier to keep track of what we've changed.

## Comment formatting
When adding comments to upstream files, use the following formats.
Generally, you should state the reason why you modified the code.

For modifying single lines of YAML:
```yaml
# TC14 - (reason)
```

For multi-line YAML changes:
```yaml
# TC14 - Begin - (reason)
(content)
(content)
# TC14 - End
```

Example:
```yaml
# TC14 - Begin - Make warops admin only
#  inhand:
#  - NukeOpsDeclarationOfWar
# TC14 - End
```


For modifying single lines of C#:
```csharp
public const int LowPressureDamage = 4; // TC14 - Revert to original value for a better MRP experience
```

For multi-line C# changes:
```csharp
public EntityUid SpawnPlayerMob(
        EntityCoordinates coordinates,
        ProtoId<JobPrototype>? job,
        HumanoidCharacterProfile? profile,
        EntityUid? station,
        EntityUid? entity = null)
    {

[rest of file]

        if (_randomizeCharacters)
        {
            profile = HumanoidCharacterProfile.RandomWithSpecies(speciesId);
        }

        // TC14 - Begin - Clown/Borg/Mime loadout names (Moved from lower in file, separated from ID processing)
        if (profile != null)
        {
            _humanoidSystem.LoadProfile(entity.Value, profile);
            _metaSystem.SetEntityName(entity.Value, profile.Name);

            if (profile.FlavorText != "" && _configurationManager.GetCVar(CCVars.FlavorText))
            {
                AddComp<DetailExaminableComponent>(entity.Value).Content = profile.FlavorText;
            }
        }
        // TC14 - End - Clown/Borg/Mime loadout names

        if (loadout != null)
        {
            EquipRoleLoadout(entity.Value, loadout, roleProto!);
        }

[rest of file]

}
```
For easier organization, you can also choose to wrap large sections of TC14-specific upstream changes in their own `#region`. Make sure to keep the comments at the top and bottom of the region, as they are used to identify the changes when searching for them.

```csharp
#region TC14 - (reason)
// TC14 - Begin - (reason)
(content)
(content)
// TC14 - End
#endregion
```

## Porting content from other forks
> [!CAUTION]
> Be sure that the content you are porting is licensed under the same license as TC14, or a compatible one. Content that comes from AGPL-licensed forks is not allowed to be ported to TC14, as it is not compatible with our license.

When porting content from other forks, make sure to separate ported content into its own namespace. This makes it easier to keep track of what content is ported from other forks, and what content is original to TC14. For example, porting content over from Sector Umbra would be placed in the `_Umbra` namespace, and porting content from Harmony would be placed in the `_Harmony` namespace.

If the ported content doesn't come in a namespace folder, create a new namespace folder with the fork name and place the content in there.

## Modifying art and sprites
We're happy to accept cool art and sprites for TC14!

We don't have any specific guidelines for new art and sprites other than:
- Does it look good?
- Does it fit the general theme of Upstream/TC14?

## Balance Changes
Changes centered around balance are brought under higher scrutiny than normal changes for the following reasons:
- Balance changes often have to modify upstream files, which make upstream merges more annoying for Maintainers to perform.
- Microbalancing is often not worth it; small changes to the balance usually do not achieve any notable effect.
- Usually, the balance change should be a complete overhaul of the mechanic instead.

Any sort of balancing that is submitted here must have a **proper lengthy justification** (**not** a 2 sentence explainer of what the balance changes do).
Even if you explain your changes, this does **not** mean your changes will automatically be merged - your PR can be closed at maintainer discretion.

## Before submitting a pull request
Before submitting a pull request, make sure to:
- Test your changes in a development environment running TC14.
  - Be sure to play around with your feature more than "if it works" as it could cause weird behavior when interacting with other features.
- Double-check your diff on git to make sure you didn't accidentally include any changes you didn't mean to.
  - Similarly, make sure you're PRing to the right place, otherwise you'll accidentally include a morbillion commits and pipe bomb someone else.
- Revert any unnecessary whitespace changes in your pull request.

## HELP I ACCIDENTALLY INCLUDED ROBUSTTOOLBOX IN MY CHANGES
```commandline
git checkout upstream/master RobustToolbox
git submodule update --init --recursive
```
This will revert the changes to the RobustToolbox submodule to the upstream version. You can then commit this change and push it to your branch.

Note that `upstream` in this instance might be different depending on your git setup. If you have a different name for the upstream repository, use that instead.

Upstream in this context is referring to https://git.tc14.space/tc14/trailblazer-colony-14.

