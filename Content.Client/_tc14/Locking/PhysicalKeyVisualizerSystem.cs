using Content.Shared._tc14.Locking.Components;
using Robust.Client.GameObjects;

namespace Content.Client._tc14.Locking;

public sealed class PhysicalKeyVisualizerSystem : VisualizerSystem<PhysicalKeyComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, PhysicalKeyComponent component, ref AppearanceChangeEvent args)
    {
        for (var i = 0; i <= 15; i++)
        {
            if (!AppearanceSystem.TryGetData<bool>(uid, (PhysicalKeyVisuals)i, out var value))
                continue;
            SpriteSystem.LayerSetVisible((uid, args.Sprite), (PhysicalKeyVisuals)i, value);
        }
    }
}
