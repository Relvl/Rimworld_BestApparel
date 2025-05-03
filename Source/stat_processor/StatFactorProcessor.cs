using System;
using System.Globalization;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class StatFactorProcessor(StatDef statDef, Func<Thing, float> func, IStatCollector collector) : AStatProcessor(statDef, collector)
{
    public override float GetStatValue(Thing thing) => func(thing);

    public override string GetStatValueFormatted(Thing thing) => $"{(GetStatValue(thing) * 100).ToString(CultureInfo.CurrentCulture)}%";
}