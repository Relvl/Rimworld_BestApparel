using System;
using System.Linq;
using BestApparel.data;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel.ui
{
    public class ThingInfoWindow : AUtilityWindow
    {
        public override Vector2 InitialSize => new Vector2(650, 800);
        private readonly Thing _thing;

        public ThingInfoWindow(MainTabWindow parent, Thing thing) : base(parent)
        {
            _thing = thing;
        }

        public override void DoWindowContents(Rect inRect)
        {
        }
    }
}