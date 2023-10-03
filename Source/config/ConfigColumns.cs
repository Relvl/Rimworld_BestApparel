using System.Collections.Generic;
using Verse;

namespace BestApparel.config;

public class ConfigColumns : IExposable
{
    private HashSet<string> _columns = new();

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
        _columns ??= new HashSet<string>();
    }

    public HashSet<string> GetColumns() => _columns;

    public bool GetColumn(string def) => _columns.Contains(def);

    public void SetColumn(string def, bool value)
    {
        if (value) _columns.Add(def);
        else _columns.Remove(def);
    }

    public void Clear() => _columns.Clear();
}