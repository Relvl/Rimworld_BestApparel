using RimWorld;
using Verse;

namespace BestApparel;

[DefOf]
public static class BestApparelStatDefOf
{
    public static StatDef MeleeWeapon_AverageArmorPenetration;

    static BestApparelStatDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(BestApparelStatDefOf));
    }
}
