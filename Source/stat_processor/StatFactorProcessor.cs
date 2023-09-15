using System;
using System.Globalization;
using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public class StatFactorProcessor : AStatProcessor
{
    private readonly Func<Thing, float> _func;

    public StatFactorProcessor(StatDef def, Func<Thing, float> func) : base(def)
    {
        _func = func;
    }

    public override float GetStatValue(Thing thing)
    {
        return _func(thing);
    }

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
    {
        return $"{(GetStatValue(thing) * 100).ToString(CultureInfo.CurrentCulture)}%";
    }
}