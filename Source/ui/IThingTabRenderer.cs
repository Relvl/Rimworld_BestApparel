using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BestApparel.ui;

public interface IThingTabRenderer
{
    string GetTabId();
    
    void DoWindowContents(ref Rect inRect);

    IEnumerable<(TranslationCache.E, Action)> GetToolbarLeft();

    IEnumerable<(TranslationCache.E, Action)> GetToolbarRight();

    void PrepareCriteria();

    void CollectContainers();

    void UpdateFilter();

    void UpdateSort();

    void DisposeContainers();

    IEnumerable<(IEnumerable<Def>, TranslationCache.E)> GetFilterData();

    IEnumerable<AStatProcessor> GetColumnData();
}