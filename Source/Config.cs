using System.Collections.Generic;
using System.Linq;
using BestApparel.ui;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BestApparel
{
    public class Config : ModSettings
    {
        public const float MaxSortingWeight = 10f;
        public const float DefaultTolerance = 0.0001f;

        public static BestApparel ModInstance;
        public static bool IsCeLoaded = ModsConfig.ActiveModsInLoadOrder.Any(m => "Combat Extended".Equals(m.Name));

        // ========================== NON storable

        public TabId SelectedTab = TabId.APPAREL;

        // ========================== Storable

        /* Do show all the things? Otherwise - only available on the workbenches. */
        public bool UseAllThings = false;

        /* */
        public bool DoLogThingsLoading = false;

        /* */
        public bool DoAutoResortOnAnyChanges = false;

        public FeatureEnableDisable ApparelLayers = new FeatureEnableDisable();
        public FeatureEnableDisable ApparelBodyParts = new FeatureEnableDisable();
        public FeatureEnableDisable ApparelCategories = new FeatureEnableDisable();
        public FeatureEnableDisable RangedTypes = new FeatureEnableDisable();
        public FeatureEnableDisable RangedCategories = new FeatureEnableDisable();

        public SortingData Sorting = new SortingData();
        public SelectedColumnsData Columns = new SelectedColumnsData();

        public void RestoreDefaultFilters()
        {
            switch (SelectedTab)
            {
                case TabId.APPAREL:
                    ApparelLayers.Clear();
                    ApparelBodyParts.Clear();
                    break;
                case TabId.RANGED:
                    RangedCategories.Clear();
                    RangedTypes.Clear();
                    break;
            }
        }

        public void RestoreDefaultColumns() // todo! по вкладкам отдельно!
        {
            switch (SelectedTab)
            {
                case TabId.APPAREL:
                    Columns.Apparel.Clear();
                    // todo defaults!
                    break;
            }
        }

        public void RestoreDefaultSortingFor(TabId tabId)
        {
            switch (SelectedTab)
            {
                case TabId.APPAREL:
                    Sorting.Apparel.Clear();
                    break;
            }
        }

        public void PrefillSorting()
        {
            foreach (var colId in Columns.Apparel)
            {
                if (!Sorting.Apparel.ContainsKey(colId))
                {
                    Sorting.Apparel[colId] = 0f;
                }
            }

            foreach (var colId in Columns.Ranged)
            {
                if (!Sorting.Ranged.ContainsKey(colId))
                {
                    Sorting.Ranged[colId] = 0f;
                }
            }

            foreach (var colId in Columns.Melee)
            {
                if (!Sorting.Melee.ContainsKey(colId))
                {
                    Sorting.Melee[colId] = 0f;
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref UseAllThings, "UseAllThings", false, true);
            Scribe_Values.Look(ref DoLogThingsLoading, "DoLogThingsLoading", false, true);
            Scribe_Values.Look(ref DoAutoResortOnAnyChanges, "DoAutoResortOnAnyChanges", false, true);

            Scribe_Deep.Look(ref Columns, "Columns");
            if (Columns == null) Columns = new SelectedColumnsData();

            Scribe_Deep.Look(ref Sorting, "SortingData");
            if (Sorting == null) Sorting = new SortingData();

            RegisterEnabledDisabledFeature(ref ApparelLayers, "ApparelLayers");
            RegisterEnabledDisabledFeature(ref ApparelBodyParts, "ApparelBodyParts");
            RegisterEnabledDisabledFeature(ref ApparelCategories, "ApparelCategories");
            RegisterEnabledDisabledFeature(ref RangedTypes, "RangedTypes");
            RegisterEnabledDisabledFeature(ref RangedCategories, "RangedTypes");
        }

        private static void RegisterHashSet<T>(ref HashSet<T> set, string name)
        {
            Scribe_Collections.Look(ref set, true, name);
            if (set == null) set = new HashSet<T>();
        }

        private static void RegisterEnabledDisabledFeature(ref FeatureEnableDisable feature, string name)
        {
            Scribe_Deep.Look(ref feature, name);
            if (feature == null) feature = new FeatureEnableDisable();
        }

        public class FeatureEnableDisable : IExposable
        {
            private HashSet<string> _disabled = new HashSet<string>();
            private HashSet<string> _enabled = new HashSet<string>();

            public void Enable(string name)
            {
                _enabled.Add(name);
                _disabled.Remove(name);
            }

            public void Disable(string name)
            {
                _enabled.Remove(name);
                _disabled.Add(name);
            }

            public void Neutral(string name)
            {
                _enabled.Remove(name);
                _disabled.Remove(name);
            }

            public MultiCheckboxState GetState(string name)
            {
                if (_enabled.Contains(name)) return MultiCheckboxState.On;
                if (_disabled.Contains(name)) return MultiCheckboxState.Off;
                return MultiCheckboxState.Partial;
            }

            public MultiCheckboxState Toggle(string name, bool reverse = false)
            {
                switch (GetState(name))
                {
                    case MultiCheckboxState.On:
                        if (reverse)
                        {
                            Neutral(name);
                            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            return MultiCheckboxState.Partial;
                        }
                        else
                        {
                            Disable(name);
                            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            return MultiCheckboxState.Off;
                        }
                    case MultiCheckboxState.Off:
                        if (reverse)
                        {
                            Enable(name);
                            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                            return MultiCheckboxState.On;
                        }
                        else
                        {
                            Neutral(name);
                            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            return MultiCheckboxState.Partial;
                        }
                    default:
                        if (reverse)
                        {
                            Disable(name);
                            SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
                            return MultiCheckboxState.Off;
                        }
                        else
                        {
                            Enable(name);
                            SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
                            return MultiCheckboxState.On;
                        }
                }
            }

            public void Clear()
            {
                _enabled.Clear();
                _disabled.Clear();
            }

            public bool IsAllowed(string name) => !_disabled.Contains(name) && _enabled.Contains(name);

            public bool IsCollectionAllowed<T>(ICollection<T> collection) where T : Def
            {
                // Every Enabled must be in the collection
                if (collection != null && _enabled.Count > 0 && !_enabled.All(e => collection.Any(c => c.defName == e))) return false;
                // No one Disabled must be in the collection
                if (collection != null && _disabled.Count > 0 && collection.Any(e => _disabled.Contains(e.defName))) return false;
                return true;
            }

            public void ExposeData()
            {
                Scribe_Collections.Look(ref _enabled, "Enabled");
                if (_enabled == null) _enabled = new HashSet<string>();
                Scribe_Collections.Look(ref _disabled, "Disabled");
                if (_disabled == null) _disabled = new HashSet<string>();
            }
        }

        public class SortingData : IExposable
        {
            public Dictionary<string, float> Apparel = new Dictionary<string, float>();
            public Dictionary<string, float> Ranged = new Dictionary<string, float>();
            public Dictionary<string, float> Melee = new Dictionary<string, float>();

            public void ExposeData()
            {
                Scribe_Collections.Look(ref Apparel, "Apparel");
                Scribe_Collections.Look(ref Ranged, "Ranged");
                Scribe_Collections.Look(ref Melee, "Melee");
            }
        }

        public class SelectedColumnsData : IExposable
        {
            public List<string> Apparel = new List<string>();
            public List<string> Ranged = new List<string>();
            public List<string> Melee = new List<string>();

            public void ExposeData()
            {
                Scribe_Collections.Look(ref Apparel, "Apparel");
                Scribe_Collections.Look(ref Ranged, "Ranged");
                Scribe_Collections.Look(ref Melee, "Melee");
            }
        }
    }
}