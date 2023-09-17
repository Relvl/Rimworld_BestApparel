using System;
using BestApparel.ui;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel;

public abstract class AStatProcessor
{
    protected static readonly StatDef DefaultStat = new();

    public readonly IStatCollector Collector;
    protected readonly StatDef Def;

    public virtual float CellWidth => 70;

    public virtual string[] ActivateWith => new string[] { };

    protected AStatProcessor(StatDef def,IStatCollector collector)
    {
        Def = def;
        Collector = collector;
    }

    public virtual StatDef GetStatDef() => Def;
    public virtual string GetDefName() => Def?.defName ?? "--unk-def-name--";
    public virtual string GetDefLabel() => Def.label;

    public abstract float GetStatValue(Thing thing);
    public abstract string GetStatValueFormatted(Thing thing);

    protected static string GetStatValueFormatted(StatDef def, float value)
    {
        return def.ValueToString(value, ToStringNumberSense.Offset, !def.formatString.NullOrEmpty());
    }

    public virtual bool IsValueDefault(Thing thing) => Math.Abs(GetStatValue(thing) - GetStatDef().defaultBaseValue) < Config.DefaultTolerance;

    public override int GetHashCode() => GetDefName().GetHashCode();

    public virtual CellData MakeCell(Thing thing) => new(this, thing);

    public virtual void RenderCell(Rect cellRect, CellData cell, IThingTabRenderer renderer)
    {
        cellRect.xMax -= 2;
        if (cell.IsEmpty) GUI.color = UIUtils.ColorWhiteA20;
        Widgets.Label(cellRect, cell.Value);
        cellRect.xMax += 2;
        foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);
    }
}