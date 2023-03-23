using System.Collections.Generic;
using Verse;

namespace BestApparel;

public interface IStatCollector
{
    IEnumerable<AStatProcessor> Collect(Thing thing);
}