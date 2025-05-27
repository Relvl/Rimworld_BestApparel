using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BestApparel;

public abstract class AThingContainer
{
    public readonly ThingDef Def;
    public readonly Thing DefaultThing;
    protected readonly string TabIdStr;
    private string _defaultTooltip;

    protected AThingContainer(ThingDef thingDef, string tabId)
    {
        Def = thingDef;
        TabIdStr = tabId;
        if (thingDef.MadeFromStuff)
            // todo! деструктуризация по материалу
            // todo! вычислить лучший материал по выбранным параметрам сортировки
            DefaultThing = ThingMaker.MakeThing(thingDef, GenStuff.DefaultStuffFor(thingDef));
        else
            DefaultThing = ThingMaker.MakeThing(thingDef);
    }

    public string DefaultTooltip
    {
        get
        {
            if (_defaultTooltip is null)
            {
                var label = GenLabel.ThingLabel(Def, DefaultThing.Stuff).CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor);
                var extras = GenLabel.LabelExtras(DefaultThing, true, true);
                var description = DefaultThing.DescriptionDetailed;
                _defaultTooltip = $"{label}{extras}\n\n{description}";
            }

            return _defaultTooltip;
        }
    }

    public CellData[] CachedCells { get; private set; } = [];

    public float CachedSortingWeight { get; private set; }

    public abstract bool CheckForFilters();

    public void CacheCells(Dictionary<AStatProcessor, (float, float)> calculated)
    {
        var list = calculated.Select(
            pair =>
            {
                var valueMin = pair.Value.Item1;
                var value = pair.Key.GetStatValue(DefaultThing);
                var valueMax = pair.Value.Item2;

                var normal = (value - valueMin) / (valueMax - valueMin);
                if (float.IsNaN(normal)) normal = 0f;

                var cell = pair.Key.MakeCell(DefaultThing);

                if (BestApparel.Config.UseSimpleDataSorting)
                {
                    if (BestApparel.Config.SimpleSorting.TryGetValue(TabIdStr, out var sorting)) cell.WeightFactor = cell.Processor.GetDefName() == sorting.First ? sorting.Second : 0;
                }
                else
                {
                    cell.WeightFactor = BestApparel.GetTabConfig(TabIdStr).Sorting.GetSorting(pair.Key.GetDefName()) + Config.MaxSortingWeight;
                }

                cell.NormalizedWeight = normal;

                return cell;
            }
        );
        if (!BestApparel.Config.DoNotSortColumns) list = list.OrderByDescending(cellData => cellData.WeightFactor).ThenBy(cellData => cellData.Processor.GetDefLabel());

        CachedCells = list.ToArray();
        CachedSortingWeight = CachedCells.Sum(c => c.NormalizedWeight * c.WeightFactor);
    }

    public override int GetHashCode()
    {
        return Def.GetHashCode();
    }

    public bool IsSearchAccept(string search)
    {
        if (string.IsNullOrEmpty(search)) return true;
        var s = search.ToLower();
        if (Def.defName.ToLower().Contains(s)) return true;
        if (Def.label.ToLower().Contains(s)) return true;
        return false;
    }
}