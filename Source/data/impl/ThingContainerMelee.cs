using System.Collections.Generic;
using BestApparel.compatibility;
using BestApparel.stat_processor;
using BestApparel.ui;
using RimWorld;
using Verse;

namespace BestApparel.data.impl;

public class ThingContainerMelee : AThingContainer
{
    public override TabId GetTabId() => TabId.Melee;

    public ThingContainerMelee(ThingDef thingDef) : base(thingDef)
    {
    }

    public override bool CheckForFilters()
    {
        if (!BestApparel.Config.Melee.Category.IsCollectionAllowed(Def.thingCategories)) return false;
        if (!BestApparel.Config.Melee.WeaponClass.IsCollectionAllowed(Def.weaponClasses)) return false;
        return true;
    }

    public override IEnumerable<AStatProcessor> CollectStats()
    {
        foreach (var def in DefDatabase<StatDef>.AllDefs)
        {
            if (def.Worker.ShouldShowFor(StatRequest.For(DefaultThing)) && !def.Worker.IsDisabledFor(DefaultThing))
            {
                var proc = new BaseStatProcessor(def);
                if (!proc.IsValueDefault(DefaultThing))
                {
                    yield return proc;
                }
            }
        }

        if (Def.equippedStatOffsets != null)
        {
            foreach (var statOffset in Def.equippedStatOffsets)
            {
                var stat = statOffset?.stat;
                if (stat == null) continue;
                var proc = new CommonStatProcessor(stat);
                if (!proc.IsValueDefault(DefaultThing))
                {
                    yield return proc;
                }
            }
        }

        if (Config.IsCeLoaded)
        {
            yield return new CommonStatProcessor(StatDef.Named("MeleePenetrationFactor"));
            yield return new CommonStatProcessor(StatDef.Named("MeleeCounterParryBonus"));
            foreach (var processor in CombatExtendedCompat.GetMeleeStats(Def)) yield return processor;
        }
        else
        {
            yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_AverageArmorPenetration);
            yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_DamageMultiplier);
            yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_CooldownMultiplier);
            yield return new CommonStatProcessor(StatDefOf.MeleeWeapon_AverageDPS);
        }
    }
}