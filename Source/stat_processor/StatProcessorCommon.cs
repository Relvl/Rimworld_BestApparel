using RimWorld;
using Verse;

namespace BestApparel.stat_processor
{
    public class StatProcessorCommon : AStatProcessor
    {
        public StatProcessorCommon(StatDef def) : base(def)
        {
        }

        public override float GetStatValue(Thing thing) => StatWorker.StatOffsetFromGear(thing, Def);

        public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
        {
            var value = GetStatValue(thing);
            return GetStatValueFormatted(Def, value, forceUnformatted);
        }

        public override int GetHashCode() => Def.GetHashCode();
    }
}