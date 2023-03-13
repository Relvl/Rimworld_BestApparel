using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.ui;
using Verse;

namespace BestApparel;

public class Config : ModSettings
{
    public const float MaxSortingWeight = 10f;
    public const float DefaultTolerance = 0.0001f;

    public static BestApparel ModInstance;
    public static readonly bool IsCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));

    // ========================== NON storable

    public TabId SelectedTab = TabId.Apparel;

    // ========================== Storable

    /* Do show all the things? Otherwise - only available on the workbenches. */
    public bool UseAllThings;

    /* */
    public bool DoLogThingsLoading;

    /* */
    public bool DoAutoResortOnAnyChanges;

    public ApparelConfig Apparel = new();
    public RangedConfig Ranged = new();
    public MeleeConfig Melee = new();
    public List<string> FittingWorn = new();

    public void RestoreDefaultFilters()
    {
        switch (SelectedTab)
        {
            case TabId.Apparel:
                Apparel.Stuff.Clear();
                Apparel.Category.Clear();
                Apparel.Layer.Clear();
                Apparel.BodyPart.Clear();
                break;
            case TabId.Ranged:
                Ranged.Stuff.Clear();
                Ranged.Category.Clear();
                Ranged.WeaponClass.Clear();
                break;
            case TabId.Melee:
                Melee.Stuff.Clear();
                Melee.Category.Clear();
                break;
        }
    }

    public void RestoreDefaultColumns()
    {
        switch (SelectedTab)
        {
            case TabId.Apparel:
                Apparel.Columns.Clear();
                break;
            case TabId.Ranged:
                Ranged.Columns.Clear();
                break;
            case TabId.Melee:
                Melee.Columns.Clear();
                break;
        }
    }

    public void RestoreDefaultSortingFor()
    {
        switch (SelectedTab)
        {
            case TabId.Apparel:
                Apparel.Sorting.Clear();
                break;
            case TabId.Ranged:
                Ranged.Sorting.Clear();
                break;
            case TabId.Melee:
                Melee.Sorting.Clear();
                break;
        }
    }

    public List<string> GetColumnsFor(TabId tabId)
    {
        return tabId switch
        {
            TabId.Apparel => Apparel.Columns,
            TabId.Ranged => Ranged.Columns,
            TabId.Melee => Melee.Columns,
            _ => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
        };
    }

    public Dictionary<string, float> GetSortingFor(TabId tabId)
    {
        return tabId switch
        {
            TabId.Apparel => Apparel.Sorting,
            TabId.Ranged => Ranged.Sorting,
            TabId.Melee => Melee.Sorting,
            _ => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
        };
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref UseAllThings, "UseAllThings", false, true);
        Scribe_Values.Look(ref DoLogThingsLoading, "DoLogThingsLoading", false, true);
        Scribe_Values.Look(ref DoAutoResortOnAnyChanges, "DoAutoResortOnAnyChanges", false, true);

        Scribe_Deep.Look(ref Apparel, "Apparel");
        Apparel ??= new ApparelConfig();
        Scribe_Deep.Look(ref Ranged, "Ranged");
        Ranged ??= new RangedConfig();
        Scribe_Deep.Look(ref Melee, "Melee");
        Melee ??= new MeleeConfig();

        Scribe_Collections.Look(ref FittingWorn, "FittingWorn");
        FittingWorn ??= new List<string>();
    }

    public class ApparelConfig : IExposable
    {
        public List<string> Columns = new();
        public Dictionary<string, float> Sorting = new();

        public FeatureEnableDisable Stuff = new();
        public FeatureEnableDisable Category = new();
        public FeatureEnableDisable Layer = new();
        public FeatureEnableDisable BodyPart = new();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Columns, "Columns");
            Scribe_Collections.Look(ref Sorting, "Sorting");
            Scribe_Deep.Look(ref Stuff, "Stuff");
            Scribe_Deep.Look(ref Category, "Category");
            Scribe_Deep.Look(ref Layer, "Layer");
            Scribe_Deep.Look(ref BodyPart, "BodyPart");

            Columns ??= new List<string>();
            Sorting ??= new Dictionary<string, float>();
            Stuff ??= new FeatureEnableDisable();
            Category ??= new FeatureEnableDisable();
            Layer ??= new FeatureEnableDisable();
            BodyPart ??= new FeatureEnableDisable();
        }
    }

    public class RangedConfig : IExposable
    {
        public List<string> Columns = new();
        public Dictionary<string, float> Sorting = new();

        public FeatureEnableDisable Stuff = new();
        public FeatureEnableDisable Category = new();
        public FeatureEnableDisable WeaponClass = new();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Columns, "Columns");
            Scribe_Collections.Look(ref Sorting, "Sorting");
            Scribe_Deep.Look(ref Stuff, "Stuff");
            Scribe_Deep.Look(ref Category, "Category");
            Scribe_Deep.Look(ref WeaponClass, "WeaponClass");

            Columns ??= new List<string>();
            Sorting ??= new Dictionary<string, float>();
            Stuff ??= new FeatureEnableDisable();
            Category ??= new FeatureEnableDisable();
            WeaponClass ??= new FeatureEnableDisable();
        }
    }

    public class MeleeConfig : IExposable
    {
        public List<string> Columns = new();
        public Dictionary<string, float> Sorting = new();

        public FeatureEnableDisable Stuff = new();
        public FeatureEnableDisable Category = new();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Columns, "Columns");
            Scribe_Collections.Look(ref Sorting, "Sorting");
            Scribe_Deep.Look(ref Stuff, "Stuff");
            Scribe_Deep.Look(ref Category, "Category");

            Columns ??= new List<string>();
            Sorting ??= new Dictionary<string, float>();
            Stuff ??= new FeatureEnableDisable();
            Category ??= new FeatureEnableDisable();
        }
    }
}