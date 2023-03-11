using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public class CommonStatProcessor : AStatProcessor
{
    public CommonStatProcessor(StatDef def) : base(def)
    {
    }

    public override float GetStatValue(Thing thing) => StatWorker.StatOffsetFromGear(thing, Def);

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => GetStatValueFormatted(Def, GetStatValue(thing), forceUnformatted);

}