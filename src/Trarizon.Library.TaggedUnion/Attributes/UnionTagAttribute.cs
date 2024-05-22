using System;

namespace Trarizon.Library.TaggedUnion.Attributes;
/// <summary>
/// Mark on a enum and provide infos for extending this enum to a tagged union,
/// use <see cref="TagVariantAttribute"/> to define specific variants
/// </summary>
/// <param name="unionTypeName">Name of generated type</param>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class UnionTagAttribute(string? unionTypeName = null) : Attribute, ITaggedUnionGeneration
{
    /// <summary>
    /// Name of generated type
    /// </summary>
    /// <remarks>
    /// If enum type name ends with "Tag" or "Kind", the default name removes the suffix,
    /// else default name is $"{EnumName}Union"
    /// </remarks>
    public string? UnionTypeName => unionTypeName;

    /// <inheritdoc/>
    public TaggedUnionGenerationOptions Options { get; set; }
}
