using System.Collections.Generic;
using BestApparel.stat_processor;
using SurvivalToolsLightCompat.stat_processor;
using SurvivalToolsLite;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.SurvivalToolsLightCompat;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class StlStuffCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        if (!thing.def.HasModExtension<StuffPropsTool>()) yield break;
        yield return new BaseStatProcessor(ST_StatDefOf.ToolEffectivenessFactor);
        foreach (var modifier in thing.def.GetModExtension<StuffPropsTool>().toolStatFactors)
            yield return new StlStuffStatProcessor(modifier.stat);
    }
}