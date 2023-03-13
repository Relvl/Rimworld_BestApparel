using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data;

public abstract class AThingContainer
{
    public abstract TabId GetTabId();

    public readonly ThingDef Def;
    public readonly Thing DefaultThing;
    public readonly string DefaultTooltip;

    public CellData[] CachedCells { get; private set; } = { };

    public float CachedSortingWeight { get; private set; }

    protected AThingContainer(ThingDef thingDef)
    {
        Def = thingDef;
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

    public abstract IEnumerable<AStatProcessor> CollectStats();

    public void CacheCells(Dictionary<AStatProcessor, (float, float)> calculated)
    {
        CachedCells = calculated.Select(
                pair =>
                {
                    var valueMin = pair.Value.Item1;
                    var value = pair.Key.GetStatValue(DefaultThing);
                    var valueMax = pair.Value.Item2;

                    var normal = (value - valueMin) / (valueMax - valueMin);
                    if (float.IsNaN(normal)) normal = 0f;

                    var sorting = BestApparel.Config.GetSortingFor(GetTabId());
                    if (!sorting.ContainsKey(pair.Key.GetDefName())) sorting[pair.Key.GetDefName()] = 0;

                    var cell = new CellData(pair.Key, DefaultThing, sorting[pair.Key.GetDefName()] + Config.MaxSortingWeight, normal);

                    cell.Tooltips.Add("BestApparel.Label.RangePercent".Translate(Math.Round(cell.NormalizedWeight * 100f, 1), cell.WeightFactor));

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