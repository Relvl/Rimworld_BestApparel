using System.Collections.Generic;
using Verse;

namespace BestApparel;

// ReSharper disable once ClassNeverInstantiated.Global -> Mod entrance
public class BestApparel : Mod
{
    public static Config Config { get; private set; } = new();
    public static List<IReloadObserver> ReloadObservers { get; } = new();

    public BestApparel(ModContentPack content) : base(content)
    {
        Config = GetSettings<Config>();
        Config.ModInstance = this;
    }

    public override string SettingsCategory() => "BestApparelConfig";
}