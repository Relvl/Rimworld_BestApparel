using System;
using System.Diagnostics.CodeAnalysis;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

/// <summary>
/// Defines main tab additional buttons (Filter, Columns, Sorting, ...)
/// </summary>
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ToolbarButtonDef : Def
{
    /// <summary>
    /// UI button order (lesser to the left)
    /// </summary>
    public int order = 0;

    /// <summary>
    /// UI position (left/right)
    /// </summary>
    public string side = "left";

    /// <summary>
    /// Reference to action (should extends AToolbarButton)
    /// </summary>
    public Type action;
}