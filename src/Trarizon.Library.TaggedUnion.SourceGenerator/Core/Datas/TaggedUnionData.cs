using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Mirror;
using Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core.Datas;
internal sealed record TaggedUnionData(
    INamedTypeSymbol TagEnumType,
    EnumDeclarationSyntax TagEnumDeclaration,
    string UnionIdentifier,
    TaggedUnionGenerationOptions GenerationOptions,
    IReadOnlyList<TagVariantData> TagVariants)
{
    public StructUnion Build()
    {
        var containingTypes = TagEnumDeclaration.Ancestors()
            .OfTypeWhile<TypeDeclarationSyntax>()
            .Select(decl => (
                decl is RecordDeclarationSyntax rds ? $"{rds.Keyword.ValueText} {rds.ClassOrStructKeyword}" : decl.Keyword.ValueText,
                $"{decl.Identifier}{decl.TypeParameterList}"
            ))
            .Reverse();

        var variants = new List<Variant>(TagVariants.Count);

        int objCount = 0;
        bool containsUnmanaged = false;
        // The final dict in Union
        var manageds = new Dictionary<ITypeSymbol, (int TypeIndex, int Count)>(SymbolEqualityComparer.Default);
        // We reuse this dict to calculate the UnionField of VariantField in BuildData()
        var managed_tmp = new Dictionary<ITypeSymbol, int>(SymbolEqualityComparer.Default);

        foreach (var variantData in TagVariants) {
            var variant = BuildData(variantData, out var objc, out var containsUnm);
            variants.Add(variant);
            objCount = Math.Max(objCount, objc);
            containsUnmanaged |= containsUnm;
            // Update count in manageds
            foreach (var kv in managed_tmp) {
                var val = manageds[kv.Key];
                if (val.Count < kv.Value) {
                    val.Count = kv.Value;
                    manageds[kv.Key] = val;
                }
            }
            managed_tmp.Clear();
        }

        return new StructUnion(
            TagEnumDeclaration.Modifiers.ToString(),
            UnionIdentifier,
            TagEnumType.ContainingNamespace,
            containingTypes,
            TagEnumType.ToFullQualifiedDisplayString(),
            variants) {
            ObjectFieldCount = objCount,
            ContainsUnmanaged = containsUnmanaged,
            ManagedFields = manageds,
            GenerationOptions = GenerationOptions,
        };

        Variant BuildData(TagVariantData data, out int objCount, out bool containsUnmanaged)
        {
            objCount = 0;
            var unmanagedCount = 0;
            var fields = new List<VariantField>(data.Fields.Length);
            foreach (var (type, identifier) in data.Fields) {
                if (type.IsReferenceType) {
                    fields.Add(new(type, identifier, UnionField.New_Class(objCount)));
                    objCount++;
                }
                else if (type.IsUnmanagedType) {
                    unmanagedCount++;                                        // 1-based index
                    fields.Add(new(type, identifier, UnionField.New_Unmanaged(unmanagedCount)));
                }
                else {
                    if (!manageds.TryGetValue(type, out var managedInfo)) {
                        // If not exist, add a new type into the dictionary.
                        // We need to add at here because we use manageds.Count as 
                        // typeIndex, which may be used in handling another field.
                        // But we needn't update count, as we update it below.
                        manageds[type] = managedInfo = (manageds.Count, 0);
                    }

                    if (!managed_tmp.TryGetValue(type, out var instanceIndex)) {
                        instanceIndex = 0;
                    }
                    fields.Add(new(type, identifier, UnionField.New_ManagedStruct(managedInfo.TypeIndex, instanceIndex)));
                    managed_tmp[type] = instanceIndex + 1;
                }
            }

            containsUnmanaged = unmanagedCount > 0;
            return new Variant(data.EnumField.Name, fields) {
                UnmanagedFieldCount = unmanagedCount,
            };
        }
    }
}
