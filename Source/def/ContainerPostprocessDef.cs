using System;
using System.Collections.Generic;
using Verse;

namespace BestApparel.def;

// ReSharper disable once UnusedType.Global
public class ContainerPostprocessDef : Def
{
    /// <summary>
    /// (lesser first)
    /// </summary>
    public int Order = 0;
    
    public List<string> Tabs = new();

    public Type Postprocessor;
}