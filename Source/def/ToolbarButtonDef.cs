using System;
using Verse;

namespace BestApparel.def;

// ReSharper disable once ClassNeverInstantiated.Global reflective: IThingTabRenderer:ctor() -> ToolbarButtons.xml
public class ToolbarButtonDef : Def
{
    /// <summary>
    /// (lesser to the left)
    /// </summary>
    public int Order = 0;

    /// <summary>
    /// 
    /// </summary>
    public string Side = "left";

    /// <summary>
    /// 
    /// </summary>
    public Type Action;
}