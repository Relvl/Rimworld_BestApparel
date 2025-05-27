using System.Collections.Generic;
using Verse;

namespace BestApparel.config;

public class ConfigColumns : IExposable
{
    private HashSet<string> _columns = [];

    public ConfigColumns()
    {
    }

    public ConfigColumns(ConfigColumns configColumns)
    {
        _columns.AddRange(configColumns._columns);
    }

    public void ExposeData()
    {
        ScribeConfig.LookHashSetString(ref _columns, "Columns");
        _columns ??= [];
    }

    public HashSet<string> GetColumns()
    {
        return _columns;
    }

    public bool GetColumn(string def)
    {
        return _columns.Contains(def);
    }

    public void SetColumn(string def, bool value)
    {
        if (value) _columns.Add(def);
        else _columns.Remove(def);
    }

    public void Clear()
    {
        _columns.Clear();
    }
}