using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BestApparel.stat_collector;

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

public class BaseStatProcessor : AStatProcessor
{
    public BaseStatProcessor(StatDef def) : base(def)
    {
    }

    public override float GetStatValue(Thing thing) => thing.GetStatValue(Def);

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false) => GetStatValueFormatted(Def, GetStatValue(thing), forceUnformatted);

    public override int GetHashCode() => Def.GetHashCode();
}