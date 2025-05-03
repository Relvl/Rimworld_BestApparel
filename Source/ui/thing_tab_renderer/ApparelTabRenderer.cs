using System.Collections.Generic;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

// ReSharper disable once ClassNeverInstantiated.Global -- instantiated by reflection: ThingTabDef.renderClass -> ThingTab:ctor
public class ApparelTabRenderer(ThingTabDef def) : DefaultThnigTabRenderer(def)
{
    public readonly HashSet<ApparelLayerDef> Layers = new();
    public readonly HashSet<BodyPartGroupDef> BodyParts = new();

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

    public override IEnumerable<(IEnumerable<Def>, TranslationCache.E, string)> GetFilterData()
    {
        yield return (BodyParts, TranslationCache.FilterBodyParts, nameof(BodyPartGroupDef));
        yield return (Layers, TranslationCache.FilterLayers, nameof(ApparelLayerDef));
        foreach (var tuple in base.GetFilterData()) yield return tuple;
    }
}