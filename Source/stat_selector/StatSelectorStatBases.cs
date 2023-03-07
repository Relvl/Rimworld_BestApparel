using RimWorld;
using Verse;

namespace BestApparel.stat_selector
{
    public class StatSelectorStatBases : IStatSelector
    {
        private readonly string _defName;
        public StatSelectorStatBases(string defName) => _defName = defName;
        public StatModifier Get(ThingDef thingDef) => thingDef.statBases.Find(it => it.stat.defName == _defName);
    }
}