using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel;

public abstract class AThingContainer
{
    public readonly string TabIdStr;

    public readonly ThingDef Def;
    public readonly Thing DefaultThing;
    public readonly string DefaultTooltip;

    public CellData[] CachedCells { get; private set; } = { };

    public float CachedSortingWeight { get; private set; }

    protected AThingContainer(ThingDef thingDef, string tabId)
    {
        Def = thingDef;
        TabIdStr = tabId;
        if (thingDef.MadeFromStuff)
        {
            // todo! деструктуризация по материалу
            // todo! вычислить лучший материал по выбранным параметрам сортировки
            DefaultThing = ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef));
        }
        else
        {
            DefaultThing = ThingMaker.MakeThing(thingDef);
        }

        DefaultTooltip = $"{DefaultThing.LabelNoParenthesisCap.AsTipTitle()}{GenLabel.LabelExtras(DefaultThing, 1, true, true)}\n\n{DefaultThing.DescriptionDetailed}";
    }

    public abstract bool CheckForFilters();

    public void CacheCells(Dictionary<AStatProcessor, (float, float)> calculated, IThingTabRenderer renderer)
    {
        CachedCells = calculated.Select(
                pair =>
                {
                    var valueMin = pair.Value.Item1;
                    var value = pair.Key.GetStatValue(DefaultThing);
                    var valueMax = pair.Value.Item2;

                    var normal = (value - valueMin) / (valueMax - valueMin);
                    if (float.IsNaN(normal)) normal = 0f;

                    var cell = pair.Key.MakeCell(DefaultThing);
                    cell.WeightFactor = BestApparel.Config.GetSorting(renderer.GetTabId(), pair.Key.GetDefName()) + Config.MaxSortingWeight;
                    cell.NormalizedWeight = normal;

                    cell.Tooltips.Add(TranslatorFormattedStringExtensions.Translate("BestApparel.Label.RangePercent", Math.Round(cell.NormalizedWeight * 100f, 1), cell.WeightFactor));

                    return cell;
                }
            )
            .OrderByDescending(cellData => cellData.WeightFactor)
            .ThenBy(cellData => cellData.DefLabel)
            .ToArray();
        CachedSortingWeight = CachedCells.Sum(c => c.NormalizedWeight * c.WeightFactor);
    }

    public override int GetHashCode() => Def.GetHashCode();

    public bool IsSearchAccept(string search)
    {
        if (string.IsNullOrEmpty(search)) return true;
        var s = search.ToLower();
        if (Def.defName.ToLower().Contains(s)) return true;
        if (Def.label.ToLower().Contains(s)) return true;
        return false;
    }
}