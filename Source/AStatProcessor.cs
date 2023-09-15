using System;
using BestApparel.ui;
using RimWorld;
using UnityEngine;
using Verse;

namespace BestApparel;

public abstract class AStatProcessor
{
    protected static readonly StatDef DefaultStat = new();
    protected readonly StatDef Def;

    public virtual float CellWidth => 70;

    public virtual string[] ActivateWith => new string[] { };

    protected AStatProcessor(StatDef def)
    {
        Def = def;
    }

    public virtual StatDef GetStatDef() => Def;
    public virtual string GetDefName() => Def?.defName ?? "--unk-def-name--";
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

    public virtual void RenderCell(Rect cellRect, CellData cell, IThingTabRenderer renderer)
    {
        if (cell.IsEmpty)
        {
            RenderEmptyCell(cellRect);
        }
        else
        {
            cellRect.xMax -= 2;
            Widgets.Label(cellRect, cell.Value);
            cellRect.xMax += 2;
            foreach (var tooltip in cell.Tooltips) TooltipHandler.TipRegion(cellRect, tooltip);
        }
    }

    protected static void RenderEmptyCell(Rect cellRect)
    {
        GUI.color = UIUtils.ColorWhiteA20;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(cellRect, "---");
        GUI.color = Color.white;
    }
}