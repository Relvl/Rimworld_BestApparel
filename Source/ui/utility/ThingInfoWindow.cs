using UnityEngine;
using Verse;

namespace BestApparel.ui.utility
{
    public class ThingInfoWindow : AUtilityWindow
    {
        public override Vector2 InitialSize => new Vector2(650, 800);
        public override bool UseBottomButtons => false;
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
}