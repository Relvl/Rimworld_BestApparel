using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BestApparel;

public static class TranslationCache
{
    private static readonly Dictionary<string, TaggedString> Translation = new();
    private static readonly Dictionary<string, Vector2> SizeCache = new();

    public static readonly E Apparel = new("BestApparel.Apparel");
    public static readonly E Ranged = new("BestApparel.Ranged");
    public static readonly E Melee = new("BestApparel.Melee");

    public static readonly E ControlUseAllThings = new("BestApparel.Control.UseAllThings");
    public static readonly E ControlUseCraftableThings = new("BestApparel.Control.UseCraftableThings");

    public static readonly E BtnResort = new("BestApparel.Btn.Resort");
    public static readonly E BtnDefaults = new("BestApparel.Btn.Defaults");
    public static readonly E BtnFilter = new("BestApparel.Btn.Filter");
    public static readonly E BtnColumns = new("BestApparel.Btn.Columns");
    public static readonly E BtnSorting = new("BestApparel.Btn.Sorting");
    public static readonly E BtnFitting = new("BestApparel.Btn.Fitting");

    public static readonly E LabelColumns = new("BestApparel.Label.Columns");
    public static readonly E LabelSorting = new("BestApparel.Label.Sorting");

    public static readonly E FilterLayers = new("BestApparel.FilterType.Layer");
    public static readonly E FilterBodyParts = new("BestApparel.FilterType.BodyPart");
    public static readonly E FilterCategory = new("BestApparel.FilterType.Category");
    public static readonly E FilterStuff = new("BestApparel.FilterType.Stuff");
    public static readonly E FilterWeaponClass = new("BestApparel.FilterType.WeaponClass");

    public static readonly E FittingWindowTitle = new("BestApparel.WindowTitle.Fitting", true, GameFont.Medium);
    public static readonly E FittingLabelSelectApparel = new("BestApparel.Label.SelectApparel");
    public static readonly E FittingBtnRemove = new("BestApparel.Fitting.Button.Remove");
    public static readonly E FittingBtnRemoveAll = new("BestApparel.Fitting.Button.RemoveAll");
    public static readonly E FittingBtnClear = new("BestApparel.Fitting.Button.Clear");
    public static readonly E FittingBtnFromPawn = new("BestApparel.Fitting.Button.FromPawn");
    public static readonly E FittingBtnAddToSlot = new("BestApparel.Button.Fitting.AddToSlot");
    public static readonly E FittingTipSlotOccupied = new("BestApparel.Tip.Fitting.SlotOccupied");
    public static readonly E FittingBtnPin = new("BestApparel.Fitting.Button.Pin");

    public static readonly E FittingLetterTotals = new("BestApparel.Fitting.Letter.Totals");
    public static readonly E FittingLetterRemove = new("BestApparel.Fitting.Letter.Totals.Remove");
    public static readonly E FittingLetterAdd = new("BestApparel.Fitting.Letter.Totals.Add");

    public static readonly E StatMeleeAvgDamage = new("BestApparel.Stat.MeleeAvgDamage");
    public static readonly E StatMeleePenetrationSharp = new("BestApparel.Stat.CE_DescSharpPenetration");
    public static readonly E StatMeleePenetrationBlunt = new("BestApparel.Stat.CE_DescBluntPenetration");

    public static readonly E BtnRangedRestoreAmmo = new("BestApparel.Btn.Ranged.RestoreAmmo");

    private static TaggedString Get(string key, params object[] args)
    {
#pragma warning disable CS0618
        if (!Translation.ContainsKey(key)) Translation[key] = key.Translate(args);
#pragma warning restore CS0618
        return Translation[key];
    }

    private static Vector2 GetSize(string key)
    {
        if (!SizeCache.ContainsKey(key)) SizeCache[key] = Text.CalcSize(Get(key));
        return SizeCache[key];
    }

    public class E
    {
        private readonly string _key;
        public readonly TaggedString Text;
        public readonly Vector2 Size;
        public readonly TaggedString Tooltip;

        public E(string key, bool hasTooltip = true, GameFont font = GameFont.Small)
        {
            Verse.Text.Font = font;
            _key = key;
            Text = Get(key);
            Size = GetSize(key);
            if (hasTooltip) Tooltip = Get($"{key}.Tooltip");
            else Tooltip = "";
        }
    }
}