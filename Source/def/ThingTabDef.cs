using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")] // reflective: IThingTabRenderer:ctor() -> ToolbarButtons.xml
public class ThingTabDef : Def
{
    /// <summary>
    ///     List of collectors def names of StatCollectorDef
    /// </summary>
    public List<Type> collectors = [];

    /// <summary>
    /// </summary>
    public Type factoryClass;

    /// <summary>
    ///     UI tab order (lesser to the left)
    /// </summary>
    public int order = 0;

    /// <summary>
    ///     Type of IThingTabRenderer - ctor must be `protected ctor(MainTabWindow)`
    /// </summary>
    public Type renderClass;

    /// <summary>
    ///     ref to ToolbarButtonDef
    /// </summary>
    public List<string> toolbar = [];
}