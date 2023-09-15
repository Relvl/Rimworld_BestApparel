using System.Collections.Generic;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.stat_collector;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class MaterialStatCollector : IStatCollector
{
    private readonly HashSet<string> _statFactorNames = new();

    public void Prepare(Thing thing)
    {
        if (thing.def.stuffProps?.statFactors is null) return;
        foreach (var factor in thing.def.stuffProps.statFactors)
        {
            _statFactorNames.Add(factor.stat.defName);
        }
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new StringStatProcessor(mat => string.Join(",", mat.def.stuffProps.categories), "Stuff_Category");
        yield return new FuncStatProcessor(mat => mat.def.stuffProps.commonality, "Commonality");

        foreach (var factorName in _statFactorNames)
        {
            yield return new StatFactorProcessor(StatDef.Named(factorName), t => t.def.stuffProps.statFactors?.Find(f => f.stat?.defName == factorName)?.value ?? 0);
        }
    }
}