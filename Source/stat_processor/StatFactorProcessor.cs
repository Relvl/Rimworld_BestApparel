using System;
using System.Globalization;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class StatFactorProcessor : AStatProcessor
{
    private readonly Func<Thing, float> _func;

    public StatFactorProcessor(StatDef statDef, Func<Thing, float> func, IStatCollector collector) : base(statDef, collector)
    {
        _func = func;
    }

    public override float GetStatValue(Thing thing)
    {
        return _func(thing);
    }

    public override string GetStatValueFormatted(Thing thing)
    {
        return $"{(GetStatValue(thing) * 100).ToString(CultureInfo.CurrentCulture)}%";
    }
}