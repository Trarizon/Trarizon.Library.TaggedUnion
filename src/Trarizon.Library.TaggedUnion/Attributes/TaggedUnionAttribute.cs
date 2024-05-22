using System;

namespace Trarizon.Library.TaggedUnion.Attributes;
/// <summary>
/// Mark a struct as Union, use a static partial method to 
/// define variants.
/// </summary>
/// <remarks>
/// write a method like:
/// <code>
/// static partial Def(
///     int A,
///     [VariantField("Name")] string B,
///     ValueTuple C,
///     (int Index, string) D,
///     ValueTuple&lt;ValueTuple> E)
/// </code>
/// will generate tagged union like:
/// <code>
/// enum Union
/// {
///     A(int Item1),
///     B(string Name),
///     C(),
///     D(int Index, string Item2),
///     E(ValueTuple Item1)
/// }
/// </code>
/// </remarks>
/// <example>
/// </example>
[AttributeUsage(AttributeTargets.Struct)]
internal sealed class TaggedUnionAttribute(string? definationMethodName = null) : Attribute, ITaggedUnionGeneration
{
    /// <summary>
    /// Name of the static partial method defines variants.
    /// </summary>
    public string? DefinationMethodName => definationMethodName;

    /// <inheritdoc/>
    public TaggedUnionGenerationOptions Options { get; set; }
}
