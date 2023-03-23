using System.Linq;
using BestApparel.def;
using UnityEngine;
using Verse;

namespace BestApparel.ui;

// ReSharper disable once UnusedType.Global, ClassNeverInstantiated.Global -> /Defs/MainWindow.xml
public class BestApparelMainTabWindow : RimWorld.MainTabWindow
{
    private ThingTab _currentTab;

    public BestApparelMainTabWindow()
    {
        // super
        doCloseX = true;
        closeOnClickedOutside = false;
        // this
        CreateTab();
    }

    private void CreateTab()
    {
        var tabDef = DefDatabase<ThingTabDef>.AllDefs.FirstOrDefault();
        if (!Config.SelectedTab.NullOrEmpty())
            tabDef = DefDatabase<ThingTabDef>.AllDefs.FirstOrDefault(d => d.defName == Config.SelectedTab) ?? tabDef;
        _currentTab?.OnTabClosed();
        _currentTab = new ThingTab(tabDef);
    }

    public override void PreOpen()
    {
        base.PreOpen();
        CreateTab();
    }

    public override void PreClose()
    {
        _currentTab?.OnTabClosed();
        _currentTab = null;
        BestApparel.Config?.Mod?.WriteSettings();
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;

        inRect.yMin += 35f;

        TabDrawer.DrawTabs(
            inRect,
            DefDatabase<ThingTabDef>.AllDefs //
                .OrderBy(d => d.order)
                .Select(
                    tabDef => new TabRecord(
                        tabDef.label,
                        () =>
                        {
                            _currentTab?.OnTabClosed();
                            _currentTab = new ThingTab(tabDef);
                        },
                        Config.SelectedTab == tabDef.defName
                    )
                )
                .ToList()
        );

        inRect.yMin += 10f;

        if (_currentTab == null) return;
        _currentTab.DoMainWindowContents(ref inRect);

        // Absolute positions here

        var label = BestApparel.Config.UseAllThings ? TranslationCache.ControlUseAllThings : TranslationCache.ControlUseCraftableThings;
        var collectTypeRect = new Rect(windowRect.width - Margin * 2 - label.Size.x - 26, 8, label.Size.x + 16, 24);
        if (Widgets.ButtonText(collectTypeRect, label.Text))
        {
            BestApparel.Config.UseAllThings = !BestApparel.Config.UseAllThings;
            _currentTab?.Reload();
        }

        TooltipHandler.TipRegion(collectTypeRect, label.Tooltip);

        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
    }
}