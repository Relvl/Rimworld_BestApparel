using System.Collections.Generic;
using BestApparel.ui;

namespace BestApparel
{
    public class ConfigElement
    {
        public TabId SelectedTab = TabId.APPAREL;

        // by default all layers in the state 'any' - including it sets changes this behaviour 
        public readonly HashSet<string> DisabledLayers = new HashSet<string>();

        public readonly HashSet<string> EnabledLayers = new HashSet<string>();

        // by default all body parts in the state 'any' - including it sets changes this behaviour 
        public readonly HashSet<string> DisabledBodyParts = new HashSet<string>();
        public readonly HashSet<string> EnabledBodyParts = new HashSet<string>();

        public void Defaults()
        {
            EnabledLayers.Clear();
            DisabledLayers.Clear();
            EnabledBodyParts.Clear();
            DisabledBodyParts.Clear();
        }

        public void Load()
        {
        }

        public void Save()
        {
        }
    }
}