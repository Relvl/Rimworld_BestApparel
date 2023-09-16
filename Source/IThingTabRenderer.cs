using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BestApparel;

public interface IThingTabRenderer
{
    string GetTabId();

    void DoWindowContents(ref Rect inRect, string searchString);

    void PrepareCriteria();

    void CollectContainers();

    void PostProcessContainer(AThingContainer container);

    void UpdateFilter();

    void UpdateSort();

    void DisposeContainers();

    IEnumerable<(IEnumerable<Def>, TranslationCache.E, string)> GetFilterData();

    IEnumerable<AStatProcessor> GetColumnData();

    HashSet<AThingContainer> GetAllContainers();
}