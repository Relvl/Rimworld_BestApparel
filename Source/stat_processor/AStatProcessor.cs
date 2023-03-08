using RimWorld;
using Verse;

namespace BestApparel.stat_processor
{
    public abstract class AStatProcessor
    {
        protected readonly StatDef Def;

        protected AStatProcessor(StatDef def)
        {
            Def = def;
        }

        public virtual StatDef GetStatDef() => Def;
        public abstract float GetStatValue(Thing thing);
        public abstract string GetStatValueFormatted(Thing thing, bool forceUnformatted = false);

        public static string GetStatValueFormatted(StatDef def, float value, bool forceUnformatted = false)
        {
            return def.ValueToString(value, ToStringNumberSense.Offset, !forceUnformatted && !def.formatString.NullOrEmpty());
        }
    }
}