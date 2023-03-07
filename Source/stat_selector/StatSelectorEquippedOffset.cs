using RimWorld;
using Verse;

namespace BestApparel.stat_selector
{
    /** @see def.equippedStatOffsets */
    public class StatSelectorEquippedOffset : IStatSelector
    {
        private readonly string _defName;
        public StatSelectorEquippedOffset(string defName) => _defName = defName;
        public StatModifier Get(ThingDef thingDef) => thingDef.equippedStatOffsets.Find(it => it.stat.defName == _defName);
    }
}