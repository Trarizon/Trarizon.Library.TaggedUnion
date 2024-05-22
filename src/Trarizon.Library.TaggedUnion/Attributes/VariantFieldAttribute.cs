using System;

namespace Trarizon.Library.TaggedUnion.Attributes;
/// <summary>
/// Provide a name for variant field when defines by static partial method
/// </summary>
/// <param name="name">Name of this field</param>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class VariantFieldAttribute(string name) : Attribute
{
    /// <summary>
    /// Name of this field
    /// </summary>
    public string Name => name;
}
