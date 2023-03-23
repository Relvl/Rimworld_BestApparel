using BestApparel.def;
using BestApparel.ui.utility;
using Verse;

namespace BestApparel.toolbar_button;

// ReSharper disable once UnusedType.Global -- reflective: ThingTab:ctor() -> ToolbarButtonDef 
public class ToolbarButtonFilters : AToolbarButton
{
    public ToolbarButtonFilters(ToolbarButtonDef def, IThingTabRenderer renderer) : base(def, renderer)
    {
    }

    public override void Action()
    {
        Find.WindowStack.TryRemove(typeof(FilterWindow));
        Find.WindowStack.Add(new FilterWindow(Renderer));
    }
}