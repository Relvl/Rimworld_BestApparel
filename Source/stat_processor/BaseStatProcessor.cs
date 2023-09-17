using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public class BaseStatProcessor : AStatProcessor
{
    public BaseStatProcessor(StatDef def, IStatCollector collector) : base(def, collector)
    {
    }

    public override float GetStatValue(Thing thing) => thing.GetStatValue(Def);

    public override string GetStatValueFormatted(Thing thing) => GetStatValueFormatted(Def, GetStatValue(thing));

    public override int GetHashCode() => Def.GetHashCode();
}