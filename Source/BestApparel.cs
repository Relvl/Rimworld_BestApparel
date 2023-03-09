using UnityEngine;
using Verse;

namespace BestApparel
{
    // ReSharper disable once ClassNeverInstantiated.Global -> Mod entrance
    public class BestApparel : Mod
    {
        public static Config Config { get; private set; } = new Config();

        public static Color COLOR_WHITE_A20 = new Color(1f, 1f, 1f, 0.2f);
        public static Color COLOR_WHITE_A50 = new Color(1f, 1f, 1f, 0.5f);

        public BestApparel(ModContentPack content) : base(content)
        {
            Config = GetSettings<Config>();
            Config.ModInstance = this;
        }

        public override string SettingsCategory() => "BestApparelConfig";
    }
}