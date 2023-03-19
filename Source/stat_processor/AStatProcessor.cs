using System;
using System.Collections.Generic;
using BestApparel.data;
using RimWorld;
using UnityEngine;
using Verse;
using MainTabWindow = BestApparel.ui.MainTabWindow;

namespace BestApparel.stat_processor;

public abstract class AStatProcessor
{
    protected static readonly StatDef DefaultStat = new();
    protected readonly StatDef Def;

    public virtual float CellWidth => 70;

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

    public override int GetHashCode() => GetDefName().GetHashCode();

    public virtual CellData MakeCell(Thing thing) => new(this, thing);

    public virtual void RenderCell(Rect cellRect, CellData cell, MainTabWindow window)
    {
        if (cell.IsEmpty)
        {
            RenderEmptyCell(cellRect);
        }
        else
        {
            Widgets.Label(cellRect, cell.Value);
            foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);
        }
    }

    protected static void RenderEmptyCell(Rect cellRect)
    {
        GUI.color = BestApparel.ColorWhiteA20;
        Widgets.Label(cellRect, "---");
        GUI.color = Color.white;
    }
}