using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BestApparel.def;
using BestApparel.ui.utility;
using UnityEngine;
using Verse;

namespace BestApparel.ui;

// ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
public class BestApparelMainTabWindow : RimWorld.MainTabWindow
{
    private readonly IReadOnlyDictionary<string, ThingTab> _thingTabs;

    public BestApparelMainTabWindow()
    {
        // super
        doCloseX = true;
        closeOnClickedOutside = false;
        // this
        _thingTabs = new ReadOnlyDictionary<string, ThingTab>(DefDatabase<ThingTabDef>.AllDefs.ToDictionary(t => t.defName, t => new ThingTab(t)));
        Config.SelectedTab = _thingTabs.FirstOrDefault().Key ?? "";
    }

    public override void PreOpen()
    {
        base.PreOpen();
        foreach (var tab in _thingTabs.Values) tab.MainWindowPreOpen();
    }

    public override void PreClose()
    {
        Find.WindowStack.TryRemove(typeof(FilterWindow));
        Find.WindowStack.TryRemove(typeof(ColumnsWindow));
        Find.WindowStack.TryRemove(typeof(SortWindow));
        Find.WindowStack.TryRemove(typeof(FittingWindow));

        foreach (var tab in _thingTabs.Values) tab.MainWindowPreClose();

        BestApparel.Config.Mod.WriteSettings();
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        inRect.yMin += 35f;
        TabDrawer.DrawTabs(inRect, _thingTabs.Values.Select(d => d.GetTabRecord()).ToList());
        inRect.yMin += 10f;

        if (_thingTabs.ContainsKey(Config.SelectedTab))
        {
            _thingTabs[Config.SelectedTab].DoMainWindowContents(ref inRect);
        }

        // Ranged -> CE tab - todo how to extend?
        // if (Config.IsCeLoaded)
        // {
        //     UIUtils.DrawButtonsRowRight(
        //         ref inRect, //
        //         (TranslationCache.BtnRangedRestoreAmmo, () => CombatExtendedCompat.OnRangedRestoreAmmoClick(DataProcessor), BestApparel.Config.SelectedTab == TabId.Ranged)
        //     );
        // }

        // Absolute positions here
        const int collectTypeWidth = 150;
        const int btnWidth = 100;

        var collectTypeRect = new Rect(windowRect.width - Margin * 2 - 10 - collectTypeWidth - btnWidth - 10, 8, collectTypeWidth, 24);
        UIUtils.RenderCheckboxLeft(
            ref collectTypeRect,
            (BestApparel.Config.UseAllThings ? TranslationCache.ControlUseAllThings : TranslationCache.ControlUseCraftableThings).Text,
            BestApparel.Config.UseAllThings,
            state =>
            {
                BestApparel.Config.UseAllThings = state;
                // DataProcessor.OnMainWindowPreOpen();
            }
        );
        TooltipHandler.TipRegion(collectTypeRect, (BestApparel.Config.UseAllThings ? TranslationCache.ControlUseAllThings : TranslationCache.ControlUseCraftableThings).Tooltip);

        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
    }
}