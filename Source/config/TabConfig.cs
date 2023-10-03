using System;
using Verse;

namespace BestApparel.config;

public class TabConfig : IExposable
{
    public readonly ConfigFilters Filters = new();
    public readonly ConfigColumns Columns = new();
    public readonly ConfigSorting Sorting = new();

    public TabConfig()
    {
    }

    public TabConfig(TabConfig tabConfig)
    {
        Filters = new ConfigFilters(tabConfig.Filters);
        Columns = new ConfigColumns(tabConfig.Columns);
        Sorting = new ConfigSorting(tabConfig.Sorting);
    }

    public void ExposeData()
    {
        Filters.ExposeData();
        Columns.ExposeData();
        Sorting.ExposeData();
    }

    public void Clear()
    {
        Sorting.Clear();
        Columns.Clear();
        Filters.Clear();
    }
}