using System.Collections.Generic;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class BaseStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        foreach (var def in DefDatabase<StatDef>.AllDefs)
        {
            if (!def.Worker.ShouldShowFor(StatRequest.For(thing))) continue;
            if (def.Worker.IsDisabledFor(thing)) continue;

            yield return new BaseStatProcessor(def, this);
        }
    }
}