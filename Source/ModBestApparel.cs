using UnityEngine;
using Verse;

namespace BestApparel
{
    [StaticConstructorOnStartup]
    public class ModBestApparel : Mod
    {
        public static Color COLOR_WHITE_A20 = new Color(1f, 1f, 1f, 0.2f);
        
        public ModBestApparel(ModContentPack content) : base(content)
        {
        }

        public override string SettingsCategory() => "BestApparel";
    }
}