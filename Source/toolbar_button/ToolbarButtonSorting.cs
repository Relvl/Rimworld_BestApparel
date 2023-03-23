using BestApparel.def;
using BestApparel.ui.utility;
using Verse;

namespace BestApparel.toolbar_button;

// ReSharper disable once UnusedType.Global -- reflective: ThingTab:ctor() -> ToolbarButtonDef 
public class ToolbarButtonSorting : AToolbarButton
{
    public ToolbarButtonSorting(ToolbarButtonDef def, IThingTabRenderer renderer) : base(def, renderer)
    {
    }

    public override void Action()
    {
        Find.WindowStack.TryRemove(typeof(SortWindow));
        Find.WindowStack.Add(new SortWindow(Renderer));
    }
}