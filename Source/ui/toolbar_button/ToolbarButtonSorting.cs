using BestApparel.ui.utility;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once UnusedType.Global -- reflective: ThingTab:ctor() -> ToolbarButtonDef 
public class ToolbarButtonSorting(ToolbarButtonDef def, IThingTabRenderer renderer) : AToolbarButton(def, renderer)
{
    public override void Action()
    {
        Find.WindowStack.TryRemove(typeof(SortWindow));
        Find.WindowStack.Add(new SortWindow(Renderer));
    }
}