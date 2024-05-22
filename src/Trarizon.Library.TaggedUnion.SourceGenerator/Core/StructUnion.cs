using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Mirror;
using Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core;
internal sealed partial record StructUnion(
    string? Modifiers,
    string TypeName,
    INamespaceSymbol ContainingNamespace,
    IEnumerable<(string Keyword,string Identifier)> ContainingTypes,
    string TagTypeFullQualifiedName,
    IReadOnlyList<Variant> Variants)
{

    public required int ObjectFieldCount { get; init; }

    public required bool ContainsUnmanaged { get; init; }

    public required IReadOnlyDictionary<ITypeSymbol, (int TypeIndex, int Count)> ManagedFields { get; init; }


    public TaggedUnionGenerationOptions GenerationOptions { get; init; }

    public string TagPropertyName { get; init; } = "Tag";

    private string? __typeNameFQ;
    public string TypeNameFullQualified
    {
        get {
            if (__typeNameFQ is null) {
                string? ns_str = null;
                if (!ContainingNamespace.IsGlobalNamespace)
                    ns_str = $"{ContainingNamespace.ToDisplayString()}.";
                __typeNameFQ = $"{ns_str}{string.Concat(ContainingTypes.Select(t => $"{t.Identifier}."))}{TypeName}";
            }
            return __typeNameFQ;
        }
    }

    public string InternalUnmanagedUnionFullQualifiedTypeName => $"{TypeNameFullQualified}.{InternalUnmanagedUnion_TypeIdentifier}";
}
