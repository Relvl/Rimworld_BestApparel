using System.Collections.Generic;
using SurvivalToolsLightCompat.stat_processor;
using SurvivalToolsLite;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.SurvivalToolsLightCompat;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThingTabRenderer:ctor
public class StlToolModCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        if (!thing.def.HasModExtension<SurvivalToolProperties>()) yield break;
        yield return new BaseStatProcessor(ST_StatDefOf.ToolEffectivenessFactor, this);
        foreach (var modifier in thing.def.GetModExtension<SurvivalToolProperties>().baseWorkStatFactors)
            yield return new StlToolStatProcessor(modifier.stat, this);
    }
}