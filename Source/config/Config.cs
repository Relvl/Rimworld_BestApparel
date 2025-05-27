using System.Collections.Generic;
using System.Linq;
using System.Xml;
using BestApparel.config;
using BestApparel.config.preset;
using UnityEngine;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

public class Config : ModSettings
{
    // Please up this version only when breaking changes in the configs
    private const int Version = 6;

    public const float MaxSortingWeight = 10f;
    public const float DefaultTolerance = 0.0001f;
    public static readonly bool IsCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));
    public static BestApparel ModInstance;
    public static string SelectedTab;

    public readonly PresetManager PresetManager = new();

    private readonly Dictionary<string, TabConfig> _tabConfig = new();
    public bool CePenetrationShortValue;
    public bool DoNotSortColumns;

    public List<string> FittingWorn = [];

    // ========================== Storable

    public float MainWindowWidth;
    public Dictionary<string, string> RangedAmmo = new();

    public Dictionary<string, Pair<string, int>> SimpleSorting = new();
    public bool UseAllThings;
    public bool UseSimpleDataSorting;

    public List<IReloadObserver> ReloadObservers { get; } = [];

    public TabConfig GetTabConfig(string tabId)
    {
        if (!_tabConfig.ContainsKey(tabId)) _tabConfig[tabId] = new TabConfig();
        return _tabConfig[tabId];
    }

    public void ApplyTabConfig(string tabId, TabConfig tabConfig)
    {
        _tabConfig[tabId] = tabConfig;
        foreach (var observer in ReloadObservers) observer.OnDataProcessorReloaded(ReloadPhase.Changed);
    }

    private void ScribeTabConfig()
    {
        if (Scribe.EnterNode("TabConfig"))
            try
            {
                switch (Scribe.mode)
                {
                    case LoadSaveMode.Saving:
                        foreach (var (tabId, tabConfig) in _tabConfig)
                            if (Scribe.EnterNode(tabId))
                                try
                                {
                                    tabConfig.ExposeData();
                                }
                                finally
                                {
                                    Scribe.ExitNode();
                                }

                        break;
                    case LoadSaveMode.LoadingVars:
                        _tabConfig.Clear();
                        foreach (XmlElement child in Scribe.loader.curXmlParent)
                            if (Scribe.EnterNode(child.Name))
                                try
                                {
                                    _tabConfig[child.Name] = new TabConfig();
                                    _tabConfig[child.Name].ExposeData();
                                }
                                finally
                                {
                                    Scribe.ExitNode();
                                }

                        break;
                }
            }
            finally
            {
                Scribe.ExitNode();
            }
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
            ScribeConfig.LookListString(ref FittingWorn, "FittingWorn");
            ScribeConfig.LookDictionary(ref RangedAmmo, "RangedSelectedAmmo");
            Scribe_Values.Look(ref DoNotSortColumns, "DoNotSortColumns");
            Scribe_Values.Look(ref UseSimpleDataSorting, "UseSimpleDataSorting");
            ScribeConfig.LookDictionary(ref SimpleSorting, "SimpleSorting");

            ScribeTabConfig();
            PresetManager.ExposeData();
        }
        else
        {
            Log.Warning("BestApparel: version upgraded, all config clear");
            Clear();
        }
    }

    private void Clear()
    {
        foreach (var (_, tabConfig) in _tabConfig) tabConfig.Clear();
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