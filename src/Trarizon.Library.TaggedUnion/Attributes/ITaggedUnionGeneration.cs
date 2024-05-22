using System;

namespace Trarizon.Library.TaggedUnion.Attributes;
internal interface ITaggedUnionGeneration
{
    /// <summary>
    /// Options indicate generator generates more members
    /// </summary>
    TaggedUnionGenerationOptions Options { get; set; }
}

/// <summary>
/// Options indicate generator generates more members
/// </summary>
[Flags]
public enum TaggedUnionGenerationOptions
{
    /// <summary>
    /// No extra options
    /// </summary>
    Default = 0,
    /// <summary>
    /// Generate properties returns ref struct, which
    /// represents variants
    /// </summary>
    RefView = 1 << 0,
    /// <summary>
    /// Implements <see cref="IDisposable"/> if any field implements
    /// </summary>
    IDisposable = 1 << 1,
    /// <summary>
    /// Implements monad match functions
    /// </summary>
    Match = 1 << 2,
}