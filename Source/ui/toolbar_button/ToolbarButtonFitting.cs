using System.Diagnostics.CodeAnalysis;
using BestApparel.ui.utility;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ToolbarButtonFitting(ToolbarButtonDef def, IThingTabRenderer renderer) : AToolbarButton(def, renderer)
{
    public override void Action()
    {
        Find.WindowStack.TryRemove(typeof(FittingWindow));
        Find.WindowStack.Add(new FittingWindow(Renderer as ApparelTabRenderer));
    }
}