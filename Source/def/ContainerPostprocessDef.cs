using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Verse;

// ReSharper disable once CheckNamespace
namespace BestApparel;

/// <summary>
///     Defines each container postprocess after calculating
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "UnassignedField.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ContainerPostprocessDef : Def
{
    /// <summary>
    ///     Postprocess apply order (lesser first)
    /// </summary>
    public int order = 0;

    /// <summary>
    ///     Postprocessor class reference
    /// </summary>
    public Type postprocessor;

    /// <summary>
    ///     Affected tab defs
    /// </summary>
    public List<string> tabs = [];
}