using RimWorld;
using Verse;

namespace BestApparel.stat_processor
{
    public class BaseStatProcessor : AStatProcessor
    {
        public BaseStatProcessor(StatDef def) : base(def)
        {
        }

        public override float GetStatValue(Thing thing) => thing.GetStatValue(Def);

        public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => GetStatValueFormatted(Def, GetStatValue(thing), forceUnformatted);
    }
}