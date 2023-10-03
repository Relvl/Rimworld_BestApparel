using BestApparel.ui.utility;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once UnusedType.Global -- reflective: ThingTab:ctor() -> ToolbarButtonDef 
public class ToolbarButtonColumns : AToolbarButton
{
    public ToolbarButtonColumns(ToolbarButtonDef def, IThingTabRenderer renderer) : base(def, renderer)
    {
    }

    public override void Action()
    {
        Find.WindowStack.TryRemove(typeof(ColumnsWindow));
        Find.WindowStack.Add(new ColumnsWindow(Renderer));
    }
}