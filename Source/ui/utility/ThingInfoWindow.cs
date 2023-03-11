using UnityEngine;
using Verse;

namespace BestApparel.ui.utility;

public class ThingInfoWindow : AUtilityWindow
{
    public override Vector2 InitialSize => new(650, 800);
    protected override bool UseBottomButtons => false;
    private readonly Thing _thing;

    public ThingInfoWindow(MainTabWindow parent, Thing thing) : base(parent)
    {
        _thing = thing;
    }

    protected override float DoWindowContentsInner(ref Rect inRect)
    {
        return 0;
    }

    protected override void OnResetClick()
    {
    }
}