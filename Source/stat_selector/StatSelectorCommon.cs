using System;
using RimWorld;
using Verse;

namespace BestApparel.stat_selector
{
    public class StatSelectorCommon : IStatSelector
    {
        private readonly Func<ThingDef, StatModifier> _selector;
        public StatSelectorCommon(Func<ThingDef, StatModifier> selector) => _selector = selector;
        public StatModifier Get(ThingDef thingDef) => _selector(thingDef);
    }
}