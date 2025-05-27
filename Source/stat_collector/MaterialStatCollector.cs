using System.Collections.Generic;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThingTabRenderer:ctor
public class MaterialStatCollector : IStatCollector
{
    private readonly HashSet<string> _statFactorNames = [];

    public void Prepare(Thing thing)
    {
        if (thing.def.stuffProps?.statFactors is null) return;
        foreach (var factor in thing.def.stuffProps.statFactors) _statFactorNames.Add(factor.stat.defName);
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new StringStatProcessor(mat => string.Join(",", mat.def.stuffProps.categories), "Stuff_Category", this);
        yield return new FuncStatProcessor(mat => mat.def.stuffProps.commonality, "Commonality", this);

        foreach (var factorName in _statFactorNames) yield return new StatFactorProcessor(StatDef.Named(factorName), t => t.def.stuffProps.statFactors?.Find(f => f.stat?.defName == factorName)?.value ?? 0, this);
    }
}