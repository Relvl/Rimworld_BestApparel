using System;
using System.Globalization;
using UnityEngine;
using Verse;

namespace BestApparel;

// ReSharper disable once ClassNeverInstantiated.Global -> Mod entrance
public class BestApparel : Mod
{
    public static Config Config { get; private set; } = new();

    public BestApparel(ModContentPack content) : base(content)
    {
        ParseHelper.Parsers<Pair<string, int>>.Register(ParsePairStringInt);
        Config = GetSettings<Config>();
        Config.ModInstance = this;
    }

    public override string SettingsCategory() => "BestApparelConfig";

    public override void DoSettingsWindowContents(Rect inRect) => Config?.DoSettingsWindowContents(inRect);

    private static Pair<string, int> ParsePairStringInt(string value)
    {
        value = value.TrimStart('(');
        value = value.TrimEnd(')');
        var strArray = value.Split(',');
        return new Pair<string, int>(Convert.ToString(strArray[0], CultureInfo.InvariantCulture), Convert.ToInt32(strArray[1], CultureInfo.InvariantCulture));
    }
}