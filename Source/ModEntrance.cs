using UnityEngine;
using Verse;

namespace BestApparel
{
    [StaticConstructorOnStartup]
    // ReSharper disable once ClassNeverInstantiated.Global -> Mod entrance
    public class ModEntrance : Mod
    {
        public static Color COLOR_WHITE_A20 = new Color(1f, 1f, 1f, 0.2f);
        
        public ModEntrance(ModContentPack content) : base(content)
        {
        }

        public override string SettingsCategory() => "BestApparel";
    }
}