using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BestApparel.stat_processor;

public abstract class AStatProcessor
{
    protected readonly StatDef Def;

    protected AStatProcessor(StatDef def)
    {
        Def = def;
    }

    public virtual StatDef GetStatDef() => Def;
    public virtual string GetDefName() => Def.defName;
    public virtual string GetDefLabel() => Def.label;

    public abstract float GetStatValue(Thing thing);
    public abstract string GetStatValueFormatted(Thing thing, bool forceUnformatted = false);

    protected static string GetStatValueFormatted(StatDef def, float value, bool forceUnformatted = false)
    {
        return def.ValueToString(value, ToStringNumberSense.Offset, !forceUnformatted && !def.formatString.NullOrEmpty());
    }

    public virtual bool IsValueDefault(Thing thing) => Math.Abs(GetStatValue(thing) - GetStatDef().defaultBaseValue) < Config.DefaultTolerance;

    public override int GetHashCode() => Def.GetHashCode();
}