using System;
using System.Collections.Generic;
using BestApparel.ui;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel;

public abstract class AStatProcessor
{
    protected static readonly StatDef DefaultStat = new();

    public readonly IStatCollector Collector;
    public readonly StatDef StatDef;

    public virtual float CellWidth => 70;

    public virtual IEnumerable<string> ActivateWith => new string[] { };

    protected AStatProcessor(StatDef statDef, IStatCollector collector)
    {
        StatDef = statDef;
        Collector = collector;
    }

    public virtual string GetDefName() => StatDef?.defName ?? "--unk-def-name--";
    public virtual string GetDefLabel() => StatDef.label;

    public abstract float GetStatValue(Thing thing);
    public abstract string GetStatValueFormatted(Thing thing);

    public virtual bool IsValueDefault(Thing thing) => Math.Abs(GetStatValue(thing) - StatDef.defaultBaseValue) < Config.DefaultTolerance;

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

    public string DebugTooltip() =>
        @$"

DefName: {GetDefName()}
Processor: {GetType().Name}
Collector: {Collector.GetType().Name}
DefaultBase: {StatDef.defaultBaseValue}
Hide at: {StatDef.hideAtValue}
Min value: {StatDef.minValue}
".Colorize(UIUtils.ColorWhiteA20);
}