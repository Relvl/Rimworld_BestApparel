using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BestApparel.config;

public class ConfigFilters : IExposable
{
    /// <summary>
    /// [category, [defName, state]]
    /// </summary>
    private Dictionary<string, Dictionary<string, bool>> _filters = new();

    public ConfigFilters()
    {
    }

    public ConfigFilters(ConfigFilters configFilters)
    {
        _filters.AddRange(configFilters._filters.ToDictionary(pair => pair.Key, pair => new Dictionary<string, bool>(pair.Value)));
    }

    public MultiCheckboxState GetFilter(string category, string defName)
    {
        if (!_filters.ContainsKey(category)) _filters[category] = new Dictionary<string, bool>();
        if (!_filters[category].ContainsKey(defName)) return MultiCheckboxState.Partial;
        return _filters[category][defName] ? MultiCheckboxState.On : MultiCheckboxState.Off;
    }

    public MultiCheckboxState ToggleFilter(string category, string defName)
    {
        var filter = _filters.ComputeIfAbsent(category, () => new Dictionary<string, bool>());
        if (!filter.ContainsKey(defName))
        {
            filter[defName] = true;
            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
            return MultiCheckboxState.On;
        }

        if (filter[defName])
        {
            filter[defName] = false;
            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
            return MultiCheckboxState.Off;
        }

        filter.Remove(defName);
        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        return MultiCheckboxState.Partial;
    }

    public bool CheckFilter<T>(List<T> defs, string category) where T : Def
    {
        foreach (var (key, value) in _filters.ComputeIfAbsent(category, () => new Dictionary<string, bool>()))
        {
            if (value)
            {
                if (defs == null || defs.Count == 0) return false;
                if (!defs.Any(d => d.defName == key)) return false;
            }
            else
            {
                if (defs == null || defs.Count == 0) continue;
                if (defs.Any(d => d.defName == key)) return false;
            }
        }

        return true;
    }

    public void SetFilter(string category, string defName, MultiCheckboxState value)
    {
        if (!_filters.ContainsKey(category)) _filters[category] = new Dictionary<string, bool>();
        if (value == MultiCheckboxState.Partial) _filters[category].Remove(defName);
        else _filters[category][defName] = value == MultiCheckboxState.On;
    }

    public void Clear() => _filters.Clear();

    public void ExposeData()
    {
        ScribeConfig.LookDictionaryDeep2(ref _filters, "Filters");
        _filters ??= new Dictionary<string, Dictionary<string, bool>>();
    }
}