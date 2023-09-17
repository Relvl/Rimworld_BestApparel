using System;
using System.Globalization;
using Verse;

namespace BestApparel.stat_processor;

public class FuncStatProcessor : AStatProcessor
{
    private readonly Func<Thing, float> _func;
    private readonly string _name;

    public FuncStatProcessor(Func<Thing, float> func, string name, IStatCollector collector) : base(DefaultStat, collector)
    {
        _func = func;
        _name = name;
    }

    public override string GetDefName() => _name;
    public override string GetDefLabel() => _name.Translate();
    public override float GetStatValue(Thing thing) => _func(thing);
    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.CurrentCulture);

    public override int GetHashCode() => _name.GetHashCode();
}