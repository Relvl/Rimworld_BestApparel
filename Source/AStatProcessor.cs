using System;
using System.Collections.Generic;
using BestApparel.ui;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel;

public abstract class AStatProcessor(StatDef statDef, IStatCollector collector)
{
    protected static readonly StatDef DefaultStat = new();

    public readonly IStatCollector Collector = collector;
    public readonly StatDef StatDef = statDef;

    public virtual float CellWidth => -1;

    public virtual IEnumerable<string> ActivateWith => [];

    public virtual string GetDefName()
    {
        return StatDef?.defName ?? "--unk-def-name--";
    }

    public virtual string GetDefLabel()
    {
        return StatDef.label;
    }

    public abstract float GetStatValue(Thing thing);
    public abstract string GetStatValueFormatted(Thing thing);

    public virtual bool IsValueDefault(Thing thing)
    {
        return Math.Abs(GetStatValue(thing) - StatDef.defaultBaseValue) < Config.DefaultTolerance;
    }

    public override int GetHashCode()
    {
        return GetDefName().GetHashCode();
    }

    public virtual CellData MakeCell(Thing thing)
    {
        return new CellData(this, thing);
    }

    public virtual void RenderCell(Rect cellRect, CellData cell, IThingTabRenderer renderer)
    {
        cellRect.xMax -= 2;
        if (cell.IsEmpty) GUI.color = UIUtils.ColorWhiteA20;
        Widgets.Label(cellRect, cell.Value);
        cellRect.xMax += 2;
        foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);
    }

    public string DebugTooltip()
    {
        return @$"

DefName: {GetDefName()}
Processor: {GetType().Name}
Collector: {Collector.GetType().Name}
DefaultBase: {StatDef.defaultBaseValue}
Hide at: {StatDef.hideAtValue}
Min value: {StatDef.minValue}
".Colorize(UIUtils.ColorWhiteA20);
    }
}