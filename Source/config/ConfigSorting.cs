using System.Collections.Generic;
using Verse;

namespace BestApparel.config;

public class ConfigSorting : IExposable
{
    private Dictionary<string, float> _sorting = new();

    public ConfigSorting()
    {
    }

    public ConfigSorting(ConfigSorting configSorting) => _sorting.AddRange(configSorting._sorting);

    public float GetSorting(string defName) => _sorting.ContainsKey(defName) ? _sorting[defName] : 0f;

    public void SetSorting(string defName, float value)
    {
        if (value == 0) _sorting.Remove(defName);
        else _sorting[defName] = value;
    }

    public void Clear() => _sorting.Clear();

    public void ExposeData()
    {
        ScribeConfig.LookDictionary(ref _sorting, "Sorting");
        _sorting.RemoveIf(s => s.Value == 0);
    }
}