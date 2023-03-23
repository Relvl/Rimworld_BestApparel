using System;
using BestApparel.def;
using BestApparel.ui;
using UnityEngine;
using Verse;

namespace BestApparel;

public class ThingTab
{
    private readonly string _tabId;
    private readonly string _caption;
    private readonly IThingTabRenderer _renderer;

    public ThingTab(ThingTabDef def)
    {
        _tabId = def.defName;
        _caption = def.caption;
        _renderer = Activator.CreateInstance(def.renderClass, def) as IThingTabRenderer;
        if (_renderer is null)
            throw new ArgumentException($"Can't instantiate renderer class ({def.renderClass?.FullName}) - should be `public ctor(ThingTabDef)`");

        _renderer.PrepareCriteria();
    }

    public TabRecord GetTabRecord() => new(_caption, () => Config.SelectedTab = _tabId, Config.SelectedTab == _tabId);

    public void DoMainWindowContents(ref Rect inRect)
    {
        // ======================== Toolbar right side
        var toolbarLeftRect = new Rect(0, inRect.y, 0, 24);
        foreach (var (label, onClick) in _renderer.GetToolbarLeft())
        {
            toolbarLeftRect.width = label.Size.x + 16;
            if (Widgets.ButtonText(toolbarLeftRect, label.Text)) onClick();
            if (label.Tooltip != "") TooltipHandler.TipRegion(toolbarLeftRect, label.Tooltip);
            toolbarLeftRect.x += toolbarLeftRect.width + 10;
        }

        // ======================== Toolbar left side
        var r = new Rect(inRect.xMax, inRect.y, 0, 24);
        foreach (var (label, onClick) in _renderer.GetToolbarRight())
        {
            r.width = label.Size.x + 16;
            r.x -= r.width + 10;
            if (Widgets.ButtonText(r, label.Text)) onClick();
            if (label.Tooltip != "") TooltipHandler.TipRegion(r, label.Tooltip);
        }

        // ======================== Line
        inRect.yMin += 34;
        UIUtils.DrawLineAtTop(ref inRect, true, 1);

        // ======================== Table
        _renderer.DoWindowContents(ref inRect);
    }

    public void MainWindowPreOpen()
    {
        _renderer.CollectContainers();
    }

    public void MainWindowPreClose()
    {
        _renderer.DisposeContainers();
    }
}