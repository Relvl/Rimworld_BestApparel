using BestApparel.def;
using BestApparel.thing_tab_renderer;
using BestApparel.ui.utility;
using Verse;

namespace BestApparel.toolbar_button;

// ReSharper disable once UnusedType.Global -- reflective: ThingTab:ctor() -> ToolbarButtonDef 
public class ToolbarButtonFitting : AToolbarButton
{
    public ToolbarButtonFitting(ToolbarButtonDef def, IThingTabRenderer renderer) : base(def, renderer)
    {
    }

    public override void Action()
    {
        Find.WindowStack.TryRemove(typeof(FittingWindow));
        Find.WindowStack.Add(new FittingWindow(Renderer as ApparelTabRenderer));
    }
}