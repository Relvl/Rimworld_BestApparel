using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BestApparel;

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

    IEnumerable<(IEnumerable<Def>, TranslationCache.E, string)> GetFilterData();

    IEnumerable<AStatProcessor> GetColumnData();
}