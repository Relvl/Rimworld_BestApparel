using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ToolbarButtonPreset(ToolbarButtonDef def, IThingTabRenderer renderer) : AToolbarButton(def, renderer)
{
    public override void Action()
    {
        var options = new List<FloatMenuOption> { new("Save current...", () => BestApparel.Config.PresetManager.MakeNewPreset(Renderer.GetTabId()), MenuOptionPriority.Low) };
        options.AddRange(BestApparel.Config.PresetManager.Presets.Where(p => p.TabId == Renderer.GetTabId()).Select(p => p.Option));
        Find.WindowStack.Add(new FloatMenu(options));
    }
}