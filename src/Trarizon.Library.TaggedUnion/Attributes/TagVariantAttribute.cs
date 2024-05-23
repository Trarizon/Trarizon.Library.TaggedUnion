using System;

namespace Trarizon.Library.TaggedUnion.Attributes;
/// <summary>
/// Provide variant infos to extend a enum field to variant
/// </summary>
/// <param name="types">Types of variant fields</param>
/// <param name="identifiers">Field identifiers</param>
[AttributeUsage(AttributeTargets.Field)]
public class TagVariantAttribute(Type[] types, string?[]? identifiers) : Attribute
{
    /// <summary>
    /// Types of variant fields
    /// </summary>
    public Type[] Types => types;

    /// <summary>
    /// Custom identifiers of variant fields
    /// </summary>
    public string?[]? Identifiers => identifiers;

    /// <summary>
    /// Set access modifiers of New_Variant() to private
    /// </summary>
    public Accessibility CreatorAccessibility { get; set; }

    /// <summary>
    /// Create a variant with default field identifiers
    /// </summary>
    /// <param name="types">Types of variant fields</param>
    public TagVariantAttribute(params Type[] types) : this(types, null) { }

    #region Constructors

    /// <summary>
    /// Create a variant with 1 field,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier)
        : this([type1], [identifier])
    { }

    /// <summary>
    /// Create a variant with 2 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2)
        : this([type1, type2], [identifier1, identifier2])
    { }

    /// <summary>
    /// Create a variant with 6 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2, Type type3, string? identifier3)
        : this([type1, type2, type3], [identifier1, identifier2, identifier3])
    { }

    /// <summary>
    /// Create a variant with 4 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2, Type type3, string? identifier3, Type type4, string? identifier4)
        : this([type1, type2, type3, type4], [identifier1, identifier2, identifier3, identifier4])
    { }

    /// <summary>
    /// Create a variant with 5 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2, Type type3, string? identifier3, Type type4, string? identifier4, Type type5, string? identifier5)
        : this([type1, type2, type3, type4, type5], [identifier1, identifier2, identifier3, identifier4, identifier5])
    { }

    /// <summary>
    /// Create a variant with 6 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2, Type type3, string? identifier3, Type type4, string? identifier4, Type type5, string? identifier5, Type type6, string? identifier6)
        : this([type1, type2, type3, type4, type5, type6], [identifier1, identifier2, identifier3, identifier4, identifier5, identifier6])
    { }

    /// <summary>
    /// Create a variant with 7 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2, Type type3, string? identifier3, Type type4, string? identifier4, Type type5, string? identifier5, Type type6, string? identifier6, Type type7, string? identifier7)
        : this([type1, type2, type3, type4, type5, type6, type7], [identifier1, identifier2, identifier3, identifier4, identifier5, identifier6, identifier7])
    { }

    /// <summary>
    /// Create a variant with 8 fields,
    /// </summary>
    public TagVariantAttribute(Type type1, string? identifier1, Type type2, string? identifier2, Type type3, string? identifier3, Type type4, string? identifier4, Type type5, string? identifier5, Type type6, string? identifier6, Type type7, string? identifier7, Type type8, string? identifier8)
        : this([type1, type2, type3, type4, type5, type6, type7, type8], [identifier1, identifier2, identifier3, identifier4, identifier5, identifier6, identifier7, identifier8])
    { }

    #endregion
}


/// <summary>
/// Mark a variant with 1 field,
/// </summary>
public sealed class TagVariantAttribute<T>(string? identifier = null)
    : TagVariantAttribute([typeof(T)], [identifier]);

/// <summary>
/// Mark a variant with 2 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2>(string? identifier1 = null, string? identifier2 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2)], [identifier1, identifier2]);

/// <summary>
/// Mark a variant with 3 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2, T3>(string? identifier1 = null, string? identifier2 = null, string? identifier3 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2), typeof(T3)], [identifier1, identifier2, identifier3]);

/// <summary>
/// Mark a variant with 4 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2, T3, T4>(string? identifier1 = null, string? identifier2 = null, string? identifier3 = null, string? identifier4 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], [identifier1, identifier2, identifier3, identifier4]);

/// <summary>
/// Mark a variant with 5 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2, T3, T4, T5>(string? identifier1 = null, string? identifier2 = null, string? identifier3 = null, string? identifier4 = null, string? identifier5 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], [identifier1, identifier2, identifier3, identifier4, identifier5]);

/// <summary>
/// Mark a variant with 6 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2, T3, T4, T5, T6>(string? identifier1 = null, string? identifier2 = null, string? identifier3 = null, string? identifier4 = null, string? identifier5 = null, string? identifier6 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)], [identifier1, identifier2, identifier3, identifier4, identifier5, identifier6]);

/// <summary>
/// Mark a variant with 7 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2, T3, T4, T5, T6, T7>(string? identifier1 = null, string? identifier2 = null, string? identifier3 = null, string? identifier4 = null, string? identifier5 = null, string? identifier6 = null, string? identifier7 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)], [identifier1, identifier2, identifier3, identifier4, identifier5, identifier6, identifier7]);

/// <summary>
/// Mark a variant with 8 field,
/// </summary>
public sealed class TagVariantAttribute<T1, T2, T3, T4, T5, T6, T7, T8>(string? identifier1 = null, string? identifier2 = null, string? identifier3 = null, string? identifier4 = null, string? identifier5 = null, string? identifier6 = null, string? identifier7 = null, string? identifier8 = null)
    : TagVariantAttribute([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)], [identifier1, identifier2, identifier3, identifier4, identifier5, identifier6, identifier7, identifier8]);
