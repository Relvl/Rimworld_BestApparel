using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BestApparel.ui;
using JetBrains.Annotations;
using Verse;

namespace BestApparel
{
    public class Config
    {
        public static readonly Config Instance = new Config();
        public static readonly float MaxSortingWeight = 10f;

        // ========================== NON storable

        public TabId SelectedTab = TabId.APPAREL;

        // ========================== Storable

        /* Do show all the things? Otherwise - only available on the workbenches. */
        public bool UseAllThings = false;
        public readonly HashSet<string> DisabledLayers = new HashSet<string>();
        public readonly HashSet<string> EnabledLayers = new HashSet<string>();
        public readonly HashSet<string> DisabledBodyParts = new HashSet<string>();
        public readonly HashSet<string> EnabledBodyParts = new HashSet<string>();

        public readonly ReadOnlyDictionary<TabId, List<string>> SelectedColumns = new ReadOnlyDictionary<TabId, List<string>>( //
            Enum.GetValues(typeof(TabId)).Cast<TabId>().ToDictionary(t => t, t => new List<string>())
        );

        public readonly ReadOnlyDictionary<TabId, Dictionary<string, float>> SortingData = new ReadOnlyDictionary<TabId, Dictionary<string, float>>( //
            Enum.GetValues(typeof(TabId)).Cast<TabId>().ToDictionary(t => t, t => new Dictionary<string, float>())
        );

        public void RestoreDefaultFilters()
        {
            EnabledLayers.Clear();
            DisabledLayers.Clear();
            EnabledBodyParts.Clear();
            DisabledBodyParts.Clear();
        }

        public void RestoreDefaultColumns() // todo! по вкладкам отдельно!
        {
            foreach (var (_, list) in SelectedColumns) list.Clear();
            /*todo! default columns:
             apparel: p-armor, b-armor, h-armor, move-speed, work-speed, social
             ranged: 
             melee:
            */
        }

        public void RestoreDefaultSortingFor(TabId tabId)
        {
            SortingData[tabId].Clear();
        }

        public void Load()
        {
        }

        public void Save()
        {
        }
    }
}