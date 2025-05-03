using System;
using System.Globalization;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class FuncStatProcessor(Func<Thing, float> func, string name, IStatCollector collector) : AStatProcessor(DefaultStat, collector)
{
    public override string GetDefName() => name;
    public override string GetDefLabel() => name.Translate();
    public override float GetStatValue(Thing thing) => func(thing);
    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.CurrentCulture);

    public override int GetHashCode() => name.GetHashCode();
}