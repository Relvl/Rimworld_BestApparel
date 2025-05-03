using System.Globalization;
using CombatExtended;
using RimWorld;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel.CombatExtendedCompat;

public class CeRangedMagazineSizeProcessor(IStatCollector collector) : AStatProcessor(StatDef.Named("MagazineCapacity"), collector)
{
    public override float GetStatValue(Thing thing) => thing.def.GetCompProperties<CompProperties_AmmoUser>()?.magazineSize ?? thing.GetStatValue(StatDef);

    public override string GetStatValueFormatted(Thing thing) => GetStatValue(thing).ToString(CultureInfo.InvariantCulture);
}