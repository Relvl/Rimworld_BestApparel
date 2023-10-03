using System;
using System.Collections.Generic;
using System.Linq;
using BestApparel.ui;
using BestApparel.ui.utility;
using UnityEngine;
using Verse;

namespace BestApparel;

public class ThingTab
{
    private readonly IThingTabRenderer _renderer;
    private readonly List<AToolbarButton> _toolbar;
    private string _searchString;

    public ThingTab(ThingTabDef def)
    {
        _renderer = Activator.CreateInstance(def.renderClass, def) as IThingTabRenderer;
        BestApparel.Config.ReloadObservers.Add(_renderer);

        if (_renderer is null)
            throw new ArgumentException($"Can't instantiate renderer class ({def.renderClass?.FullName}) - should be `public ctor(ThingTabDef)`");

        _toolbar = def.toolbar.Select(
                d =>
                {
                    var buttonDef = DefDatabase<ToolbarButtonDef>.GetNamed(d);
                    if (buttonDef is null)
                        throw new ArgumentException($"Can't instantiate toolbar button class - BestApparel.ToolbarButtonDef[defName='{d}'] not found");
                    return buttonDef;
                }
            )
            .OrderBy(d => d.order)
            .Select(
                buttonDef =>
                {
                    if (Activator.CreateInstance(buttonDef.action, buttonDef, _renderer) is not AToolbarButton inst)
                        throw new ArgumentException(
                            $"Can't instantiate toolbar button class ({buttonDef?.action?.FullName}) - should be `public ctor(ToolbarButtonDef, IThingTabRenderer)`"
                        );
                    return inst;
                }
            )
            .ToList();

        Config.SelectedTab = def.defName;
        Reload();
    }

    public void DoMainWindowContents(ref Rect inRect)
    {
        // ======================== Toolbar 
        var btnRect = new Rect(0, inRect.y, 0, 24);
        foreach (var button in _toolbar.Where(a => a.IsLeftSide())) button.Render(ref btnRect);

        // search
        const int searchWidth = 100;
        var r = new Rect(inRect.width - 24, inRect.y, 24, 24);
        GUI.DrawTexture(r, TexButton.Search);
        GUI.SetNextControlName($"UI.BestApparel.{GetType().Name}.Search");
        r.x -= searchWidth + 4;
        r.width = searchWidth;
        var searchString = Widgets.TextField(r, _searchString, 15);
        _searchString = searchString;

        // presets

        // right buttons
        btnRect.x = inRect.xMax - r.width - 28;
        btnRect.y = inRect.y;
        btnRect.width = 0;
        foreach (var button in _toolbar.Where(a => a.IsRightSide())) button.Render(ref btnRect);

        // ======================== Line
        inRect.yMin += 34;
        UIUtils.DrawLineAtTop(ref inRect, true, 1);

        // ======================== Table
        _renderer.DoWindowContents(ref inRect, searchString);
    }

    public void Reload() => _renderer.CollectContainers();

    public void OnTabClosed()
    {
        Find.WindowStack.TryRemove(typeof(FilterWindow));
        Find.WindowStack.TryRemove(typeof(ColumnsWindow));
        Find.WindowStack.TryRemove(typeof(SortWindow));
        Find.WindowStack.TryRemove(typeof(FittingWindow));

        BestApparel.Config.ReloadObservers.Remove(_renderer);
        _renderer.DisposeContainers();
    }
}