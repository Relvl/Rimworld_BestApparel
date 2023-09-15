using System;
using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public class StringStatProcessor : AStatProcessor
{
    private readonly Func<Thing, string> _func;
    private readonly string _name;

    public StringStatProcessor(Func<Thing, string> func, string name) : base(DefaultStat)
    {
        _func = func;
        _name = name;
    }

    public override string GetDefName() => _name;
    public override string GetDefLabel() => _name.Translate();

    public override float GetStatValue(Thing thing)
    {
        return 0;
    }

    public override string GetStatValueFormatted(Thing thing, bool forceUnformatted = false)
    {
        return _func(thing);
    }

    public override int GetHashCode() => _name.GetHashCode();
}