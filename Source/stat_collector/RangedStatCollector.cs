using System.Collections.Generic;
using System.Linq;
using BestApparel.stat_processor;
using Verse;

namespace BestApparel.stat_collector;

// ReSharper disable once UnusedType.Global -- reflection: DefaultThnigTabRenderer:ctor
public class RangedStatCollector : IStatCollector
{
    public void Prepare(Thing thing)
    {
    }

    public IEnumerable<AStatProcessor> Collect(Thing thing)
    {
        yield return new FuncStatProcessor(weapon => weapon.def.Verbs.FirstOrDefault()?.defaultProjectile?.projectile?.GetDamageAmount(weapon) ?? 0, "Damage", this);
        yield return new FuncStatProcessor(weapon => weapon.def.Verbs.FirstOrDefault()?.range ?? 0, "Range", this);
    }
}