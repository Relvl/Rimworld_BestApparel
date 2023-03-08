using RimWorld;
using Verse;

namespace BestApparel.stat_processor
{
    public class StatProcessorBaseStat : AStatProcessor
    {
        public StatProcessorBaseStat(StatDef def) : base(def)
        {
        }

        public override float GetStatValue(Thing thing)
        {
            return thing.GetStatValue(Def);
        }

        public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
        {
            var value = GetStatValue(thing);
            return Def.ValueToString(value, ToStringNumberSense.Absolute, !forceUnformatted);
        }
    }
}