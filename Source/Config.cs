using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BestApparel.ui;
using JetBrains.Annotations;
using Verse;

namespace BestApparel
{
    public class Config : ModSettings
    {
        public static readonly float MaxSortingWeight = 10f;
        public static BestApparel ModInstance;

        // ========================== NON storable

        public TabId SelectedTab = TabId.APPAREL;

        // ========================== Storable

        /* Do show all the things? Otherwise - only available on the workbenches. */
        public bool UseAllThings = false;

        public HashSet<string> DisabledLayers = new HashSet<string>();
        public HashSet<string> EnabledLayers = new HashSet<string>();
        public HashSet<string> DisabledBodyParts = new HashSet<string>();
        public HashSet<string> EnabledBodyParts = new HashSet<string>();

        public SortingData Sorting = new SortingData();
        public SelectedColumnsData Columns = new SelectedColumnsData();

        public void RestoreDefaultFilters()
        {
            EnabledLayers.Clear();
            DisabledLayers.Clear();
            EnabledBodyParts.Clear();
            DisabledBodyParts.Clear();
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

        public override void ExposeData()
        {
            Scribe_Values.Look(ref UseAllThings, "UseAllThings", false, true);

            Scribe_Collections.Look(ref DisabledLayers, true, "DisabledLayers");
            Scribe_Collections.Look(ref EnabledLayers, true, "EnabledLayers");
            Scribe_Collections.Look(ref DisabledBodyParts, true, "DisabledBodyParts");
            Scribe_Collections.Look(ref EnabledBodyParts, true, "EnabledBodyParts");

            Scribe_Deep.Look(ref Columns, "Columns");
            if (Columns == null) Columns = new SelectedColumnsData();

            Scribe_Deep.Look(ref Sorting, "SortingData");
            if (Sorting == null) Sorting = new SortingData();
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