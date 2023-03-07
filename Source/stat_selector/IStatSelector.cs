using RimWorld;
using Verse;

namespace BestApparel.stat_selector
{
    public interface IStatSelector
    {
        StatModifier Get(ThingDef thingDef);
    }
}