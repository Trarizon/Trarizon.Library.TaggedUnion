using Microsoft.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core;
internal sealed record VariantField(
    ITypeSymbol Type, 
    string Name, 
    UnionField UnionField);
