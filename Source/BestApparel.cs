using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BestApparel;

// ReSharper disable once ClassNeverInstantiated.Global -> Mod entrance
public class BestApparel : Mod
{
    public static Config Config { get; private set; } = new();

    public static Color ColorWhiteA20 = new(1f, 1f, 1f, 0.2f);
    public static Color ColorWhiteA50 = new(1f, 1f, 1f, 0.5f);

    public BestApparel(ModContentPack content) : base(content)
    {
        Config = GetSettings<Config>();
        Config.ModInstance = this;
    }

    public override string SettingsCategory() => "BestApparelConfig";
}