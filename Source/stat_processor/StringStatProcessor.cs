using System;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class StringStatProcessor(Func<Thing, string> func, string name, IStatCollector collector) : AStatProcessor(DefaultStat, collector)
{
    public override string GetDefName() => name;
    public override string GetDefLabel() => name.Translate();

    public override float GetStatValue(Thing thing) => 0;

    public override string GetStatValueFormatted(Thing thing) => func(thing);

    public override int GetHashCode() => name.GetHashCode();
}