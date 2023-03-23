using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BestApparel;

public class Config : ModSettings
{
    // Please up this version only when breaking changes in the configs
    private const int Version = 3;

    public const float MaxSortingWeight = 10f;
    public const float DefaultTolerance = 0.0001f;

    public static BestApparel ModInstance;

    public static readonly bool IsCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));

    public static string SelectedTab;

    // ========================== Storable

    /* Do show all the things? Otherwise - only available on the workbenches. */
    public bool UseAllThings;

    public List<string> FittingWorn = new();
    public Dictionary<string, string> RangedAmmo = new();

    private Dictionary<string, Dictionary<string, float>> _sorting = new();
    private Dictionary<string, List<string>> _columns = new();
    private Dictionary<string, Dictionary<string, bool>> _filters = new();

    public void ClearFilters(string tabId) => _filters.ComputeIfAbsent(tabId, () => new Dictionary<string, bool>())?.Clear();

    public void ClearColumns(string tabId) => _columns.ComputeIfAbsent(tabId, () => new List<string>()).Clear();

    public void ClearSorting(string tabId) => _sorting.ComputeIfAbsent(tabId, () => new Dictionary<string, float>())?.Clear();

    public float GetSorting(string tabId, string defName) => _sorting.ComputeIfAbsent(tabId, () => new Dictionary<string, float>())?.ComputeIfAbsent(defName, () => 0) ?? 0f;

    public void SetSorting(string tabId, string defName, float value)
    {
        // todo! удалять сортинги, если 0
        if (!_sorting.ContainsKey(tabId)) _sorting[tabId] = new Dictionary<string, float>();
        _sorting[tabId][defName] = value;
    }

    public IReadOnlyList<string> GetColumns(string tabId) => _columns.ComputeIfAbsent(tabId, () => new List<string>());

    public bool GetColumn(string tabId, string def) => _columns.ComputeIfAbsent(tabId, () => new List<string>()).Contains(def);

    public void SetColumn(string tabId, string def, bool value)
    {
        if (value) _columns.ComputeIfAbsent(tabId, () => new List<string>()).Add(def);
        else _columns.ComputeIfAbsent(tabId, () => new List<string>()).Remove(def);
    }

    public MultiCheckboxState GetFilter(string tabId, string defName)
    {
        if (!_filters.ContainsKey(tabId)) _filters[tabId] = new Dictionary<string, bool>();
        if (!_filters[tabId].ContainsKey(defName)) return MultiCheckboxState.Partial;
        return _filters[tabId][defName] ? MultiCheckboxState.On : MultiCheckboxState.Off;
    }

    public MultiCheckboxState ToggleFilter(string tabId, string defName)
    {
        var filter = _filters.ComputeIfAbsent(tabId, () => new Dictionary<string, bool>());
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

    public bool CheckFilter<T>(string tabId, List<T> defs) where T : Def
    {
        var filter = _filters.ComputeIfAbsent(tabId, () => new Dictionary<string, bool>());
        foreach (var (key, value) in filter)
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

    public void SetFilter(string tabId, string defName, MultiCheckboxState value)
    {
        if (!_filters.ContainsKey(tabId)) _filters[tabId] = new Dictionary<string, bool>();
        if (value == MultiCheckboxState.Partial) _filters[tabId].Remove(defName);
        else _filters[tabId][defName] = value == MultiCheckboxState.On;
    }

    public override void ExposeData()
    {
        var version = Version;
        Scribe_Values.Look(ref version, "Version");
        if (version == Version)
        {
            Scribe_Values.Look(ref UseAllThings, "UseAllThings", false, true);
            Scribe_Config.LookDeepDictionary(ref _sorting, "Sorting");
            Scribe_Config.LookListDictionary(ref _columns, "Columns");
            Scribe_Config.LookDeepDictionary(ref _filters, "Filters");
            Scribe_Config.LookStringList(ref FittingWorn, "FittingWorn");
            Scribe_Config.LookDictionary(ref RangedAmmo, "RangedSelectedAmmo");
        }
        else
        {
            Log.Warning("BestApparel: version upgraded, all config clear");
            Clear();
            ModInstance.WriteSettings();
        }
    }

    private void Clear()
    {
        _sorting.Clear();
        _columns.Clear();
        _filters.Clear();
        FittingWorn.Clear();
        RangedAmmo.Clear();
    }
}