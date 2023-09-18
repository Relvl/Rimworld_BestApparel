using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestApparel;

public class Config : ModSettings
{
    // Please up this version only when breaking changes in the configs
    private const int Version = 5;
    public static readonly bool IsCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));

    public const float MaxSortingWeight = 10f;
    public const float DefaultTolerance = 0.0001f;

    public static BestApparel ModInstance;

    public static string SelectedTab;

    // ========================== Storable

    public float MainWindowWidth;
    public bool CePenetrationShortValue;

    /* Do show all the things? Otherwise - only available on the workbenches. */
    public bool UseAllThings;

    public bool DoNotSortColumns = false;

    public bool UseSimpleDataSorting = false;

    public List<string> FittingWorn = new();
    public Dictionary<string, string> RangedAmmo = new();

    private Dictionary<string, Dictionary<string, float>> _sorting = new();
    private Dictionary<string, HashSet<string>> _columns = new();

    /// <summary>
    /// [tabId, [category, [defName, state]]]
    /// </summary>
    private Dictionary<string, Dictionary<string, Dictionary<string, bool>>> _filters = new();

    public Dictionary<string, Pair<string, int>> SimpleSorting = new();

    public void ClearFilters(string tabId) => _filters.ComputeIfAbsent(tabId, () => new Dictionary<string, Dictionary<string, bool>>())?.Clear();

    public void ClearColumns(string tabId) => _columns.ComputeIfAbsent(tabId, () => new HashSet<string>()).Clear();

    public void ClearSorting(string tabId) => _sorting.ComputeIfAbsent(tabId, () => new Dictionary<string, float>())?.Clear();

    public float GetSorting(string tabId, string defName) => _sorting.ComputeIfAbsent(tabId, () => new Dictionary<string, float>())?.ComputeIfAbsent(defName, () => 0) ?? 0f;

    public void SetSorting(string tabId, string defName, float value)
    {
        // todo! удалять сортинги, если 0
        if (!_sorting.ContainsKey(tabId)) _sorting[tabId] = new Dictionary<string, float>();
        _sorting[tabId][defName] = value;
    }

    public HashSet<string> GetColumns(string tabId) => _columns.ComputeIfAbsent(tabId, () => new HashSet<string>());

    public bool GetColumn(string tabId, string def) => _columns.ComputeIfAbsent(tabId, () => new HashSet<string>()).Contains(def);

    public void SetColumn(string tabId, string def, bool value)
    {
        if (value)
            _columns.ComputeIfAbsent(tabId, () => new HashSet<string>()).Add(def);
        else
            _columns.ComputeIfAbsent(tabId, () => new HashSet<string>()).Remove(def);
    }

    public MultiCheckboxState GetFilter(string tabId, string category, string defName)
    {
        if (!_filters.ContainsKey(tabId)) _filters[tabId] = new Dictionary<string, Dictionary<string, bool>>();
        if (!_filters[tabId].ContainsKey(category)) _filters[tabId][category] = new Dictionary<string, bool>();
        if (!_filters[tabId][category].ContainsKey(defName)) return MultiCheckboxState.Partial;
        return _filters[tabId][category][defName] ? MultiCheckboxState.On : MultiCheckboxState.Off;
    }

    public MultiCheckboxState ToggleFilter(string tabId, string category, string defName)
    {
        var filter = _filters //
            .ComputeIfAbsent(tabId, () => new Dictionary<string, Dictionary<string, bool>>())
            .ComputeIfAbsent(category, () => new Dictionary<string, bool>());
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

    public bool CheckFilter<T>(string tabId, List<T> defs, string category) where T : Def
    {
        var filter = _filters //
            .ComputeIfAbsent(tabId, () => new Dictionary<string, Dictionary<string, bool>>())
            .ComputeIfAbsent(category, () => new Dictionary<string, bool>());
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

    public void SetFilter(string tabId, string category, string defName, MultiCheckboxState value)
    {
        if (!_filters.ContainsKey(tabId)) _filters[tabId] = new Dictionary<string, Dictionary<string, bool>>();
        if (!_filters[tabId].ContainsKey(category)) _filters[tabId][category] = new Dictionary<string, bool>();
        if (value == MultiCheckboxState.Partial) _filters[tabId][category].Remove(defName);
        else _filters[tabId][category][defName] = value == MultiCheckboxState.On;
    }

    public override void ExposeData()
    {
        var version = Version;
        Scribe_Values.Look(ref version, "Version");
        if (version == Version)
        {
            Scribe_Values.Look(ref MainWindowWidth, "MainWindowWidth");
            Scribe_Values.Look(ref CePenetrationShortValue, "CePenetrationShortValue");
            Scribe_Values.Look(ref UseAllThings, "UseAllThings", false, true);
            Scribe_Config.LookDictionaryDeep2(ref _sorting, "Sorting");
            Scribe_Config.LookDictionaryHashSet(ref _columns, "Columns");
            Scribe_Config.LookDictionaryDeep3(ref _filters, "Filters");
            Scribe_Config.LookListString(ref FittingWorn, "FittingWorn");
            Scribe_Config.LookDictionary(ref RangedAmmo, "RangedSelectedAmmo");
            Scribe_Values.Look(ref DoNotSortColumns, "DoNotSortColumns");
            Scribe_Values.Look(ref UseSimpleDataSorting, "UseSimpleDataSorting");
            Scribe_Config.LookDictionary(ref SimpleSorting, "SimpleSorting");
        }
        else
        {
            Log.Warning("BestApparel: version upgraded, all config clear");
            Clear();
        }
    }

    private void Clear()
    {
        _sorting.Clear();
        _columns.Clear();
        _filters.Clear();
        FittingWorn.Clear();
        RangedAmmo.Clear();
        DoNotSortColumns = false;
        UseSimpleDataSorting = false;
    }

    public void DoSettingsWindowContents(Rect inRect)
    {
        var list = new Listing_Standard(GameFont.Small);
        list.Begin(inRect);
        list.CheckboxLabeled(TranslationCache.DoNotSortColumns.Text, ref DoNotSortColumns);
        list.CheckboxLabeled(TranslationCache.UseSimpleDataSorting.Text, ref UseSimpleDataSorting);
        if (IsCeLoaded)
            list.CheckboxLabeled(TranslationCache.CePenetrationShortValue.Text, ref CePenetrationShortValue);
        list.End();
    }
}