using System;
using System.Collections.Generic;
using Verse;

namespace BestApparel.def;

// ReSharper disable once ClassNeverInstantiated.Global
public class ThingTabDef : Def
{
    /// <summary>
    /// UI tab order (lesser to the left)
    /// </summary>
    public int order = 0;

    /// <summary>
    /// Type of IThingTabRenderer - ctor must be `protected ctor(MainTabWindow)`
    /// </summary>
    public Type renderClass;

    /// <summary>
    /// 
    /// </summary>
    public Type factoryClass;

    /// <summary>
    /// List of collectors def names of StatCollectorDef
    /// </summary>
    public List<Type> collectors = new();

    /// <summary>
    /// ref to ToolbarButtonDef
    /// </summary>
    public List<string> toolbar = new();
}