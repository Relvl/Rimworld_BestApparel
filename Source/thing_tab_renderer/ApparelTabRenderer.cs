using System;
using System.Collections.Generic;
using BestApparel.def;
using BestApparel.ui.utility;
using Verse;

namespace BestApparel.thing_tab_renderer;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by reflection: ThingTabDef.renderClass -> ThingTab:ctor
public class ApparelTabRenderer : DefaultThnigTabRenderer
{
    public readonly HashSet<ApparelLayerDef> Layers = new();
    public readonly HashSet<BodyPartGroupDef> BodyParts = new();

    public ApparelTabRenderer(ThingTabDef def) : base(def)
    {
    }

    public override IEnumerable<(TranslationCache.E, Action)> GetToolbarRight()
    {
        yield return (TranslationCache.BtnFitting, () =>
        {
            Find.WindowStack.TryRemove(typeof(FittingWindow));
            Find.WindowStack.Add(new FittingWindow(this));
        });
    }

    public override void PrepareCriteria()
    {
        Layers.Clear();
        BodyParts.Clear();
        base.PrepareCriteria();
    }

    protected override void PrepareCriteriaEach(ThingDef def)
    {
        base.PrepareCriteriaEach(def);
        if (def.apparel?.layers != null) Layers.AddRange(def.apparel.layers);
        if (def.apparel?.bodyPartGroups != null) BodyParts.AddRange(def.apparel.bodyPartGroups);
    }
}