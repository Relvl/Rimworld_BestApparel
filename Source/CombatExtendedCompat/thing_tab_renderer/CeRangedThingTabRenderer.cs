using System;
using System.Collections.Generic;
using BestApparel.def;
using BestApparel.thing_tab_renderer;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

// ReSharper disable once ClassNeverInstantiated.Global, UnusedType.Global -- instantiated by reflection: ThingTabDef.renderClass -> ThingTab:ctor
public class CeRangedThingTabRenderer : DefaultThnigTabRenderer
{
    public CeRangedThingTabRenderer(ThingTabDef def) : base(def)
    {
    }

    public override IEnumerable<(TranslationCache.E, Action)> GetToolbarRight()
    {
        yield return (TranslationCache.BtnRangedRestoreAmmo, () => { Log.Warning("BA: restore default ammo"); });
    }
}