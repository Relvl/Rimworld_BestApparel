using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BestApparel.ui;
using Verse;

namespace BestApparel
{
    public class ConfigElement
    {
        public TabId SelectedTab = TabId.APPAREL;

        public readonly HashSet<string> DisabledLayers = new HashSet<string>();
        public readonly HashSet<string> EnabledLayers = new HashSet<string>();
        public readonly HashSet<string> DisabledBodyParts = new HashSet<string>();
        public readonly HashSet<string> EnabledBodyParts = new HashSet<string>();

        public readonly ReadOnlyDictionary<TabId, List<string>> SelectedColumns = new ReadOnlyDictionary<TabId, List<string>>( //
            Enum.GetValues(typeof(TabId)).Cast<TabId>().ToDictionary(t => t, t => new List<string>())
        );

        public void Defaults()
        {
            EnabledLayers.Clear();
            DisabledLayers.Clear();
            EnabledBodyParts.Clear();
            DisabledBodyParts.Clear();

            foreach (var (_, list) in SelectedColumns) list.Clear();
            /*todo! default columns:
             apparel: p-armor, b-armor, h-armor, move-speed, work-speed, social
             ranged: 
             melee:
            */
        }

        public void Load()
        {
        }

        public void Save()
        {
        }
    }
}