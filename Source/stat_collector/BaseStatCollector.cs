using System.Collections.Generic;
using BestApparel.stat_processor;
using RimWorld;
using Verse;

namespace BestApparel.stat_collector;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class BaseStatCollector : IStatCollector
{
    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        foreach (var def in DefDatabase<StatDef>.AllDefs)
        {
            if (def.Worker.ShouldShowFor(StatRequest.For(thing)) && !def.Worker.IsDisabledFor(thing))
            {
                var proc = new BaseStatProcessor(def);
                if (!proc.IsValueDefault(thing))
                {
                    yield return proc;
                }
            }
        }
    }
}