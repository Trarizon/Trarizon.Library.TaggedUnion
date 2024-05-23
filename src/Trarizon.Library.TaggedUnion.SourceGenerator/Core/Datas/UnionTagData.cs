using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Mirror;
using Trarizon.Library.TaggedUnion.SourceGenerator.Helpers;
using Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core.Datas;
internal sealed record UnionTagData(
    INamedTypeSymbol TagEnumType,
    EnumDeclarationSyntax TagEnumDeclaration,
    string UnionIdentifier,
    IReadOnlyList<TagVariantData> TagVariants)
{
    public TaggedUnionGenerationOptions GenerationOptions { get; init; }

    public CreatorAccessibility CreatorAccessibility { get; init; }


    public static ParseResult<UnionTagData> Parse(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetNode is not EnumDeclarationSyntax tagTypeSyntax)
            return default;
        if (context.TargetSymbol is not INamedTypeSymbol { TypeKind: TypeKind.Enum } tagTypeSymbol)
            return default;
        if (context.Attributes is not [var unionTagAttr])
            return default;

        var unionTypeIdentifier = unionTagAttr.GetConstructorArgument<string>(Literals.UnionTagAttribute_UnionTypeName_CtorIndex).Value
            ?? GetDefaultGeneratedUnionTypeIdentifier(tagTypeSymbol);

        if (!ValidationUtil.IsValidIdentifier(unionTypeIdentifier)) {
            return new DiagnosticData(Literals.DiagnosticDescriptors.InvalidIdentifier,
                tagTypeSyntax.Identifier.GetLocation(),
                unionTypeIdentifier);
        }

        var variantResult = tagTypeSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Select(Result<TagVariantData, IFieldSymbol> (field) =>
            {
                var attr = field.GetAttributes()
                .Where(attr => attr.AttributeClass?.ToDisplayString(MoreSymbolDisplayFormats.DefaultWithoutGeneric) is Literals.TagVariantAttribute_TypeName)
                .FirstOrDefault();
                if (attr is null)
                    return new TagVariantData(field);

                var tv = TagVariantData.FromAttribute(attr, field);
                if (tv is null)
                    return Result.Failed<TagVariantData, IFieldSymbol>(field);
                else
                    return tv;
            }).ToList();

        if (variantResult.Any(r => r.Failed))
            return default;

        token.ThrowIfCancellationRequested();

        var variants = variantResult.Select(r => r.Value).ToList();

        // check field identifiers
        {
            var errs = variants.WhereSelect(variant =>
            {
                var errs = variant.Fields.Where(tpl => !ValidationUtil.IsValidIdentifier(tpl.Identifier));
                if (errs.Any()) {
                    return new DiagnosticData(
                        Literals.DiagnosticDescriptors.InvalidIdentifier,
                        variant.EnumField.DeclaringSyntaxReferences[0],
                        string.Join(", ", errs));
                }
                if (!variant.Fields.IsDistinctBy(f => f.Identifier)) {
                    return new DiagnosticData(
                        Literals.DiagnosticDescriptors.VariantFieldNameRepeat,
                        variant.EnumField.DeclaringSyntaxReferences[0]);
                }
                return default(Optional<DiagnosticData>);
            });
            if (errs.ToListIfAny() is { } errsList)
                return errsList;
        }
        // check enum constant values
        {
            var errs = variants
                .DuplicatesBy(v => v.EnumField.ConstantValue)
                .Select(v => new DiagnosticData(Literals.DiagnosticDescriptors.VariantFieldNameRepeat, v.EnumField.DeclaringSyntaxReferences[0]));
            if (errs.ToListIfAny() is { } errsList)
                return errsList;
        }
        // warn 0 value
        var warnings = new List<DiagnosticData>();
        {
            var zero = variants.FirstOrDefault(v => ValidationHelpers.IsZeroEnumValue(v.EnumField) && v.Fields.Count > 0);
            if (zero is not null) {
                warnings.Add(new DiagnosticData(
                    Literals.DiagnosticDescriptors.NoZeroVariantField,
                    zero.EnumField.DeclaringSyntaxReferences[0]));
            }
        }

        token.ThrowIfCancellationRequested();

        return new(new UnionTagData(tagTypeSymbol, tagTypeSyntax, unionTypeIdentifier, variants) {
            GenerationOptions = unionTagAttr.GetNamedArgument<TaggedUnionGenerationOptions>(Literals.ITaggedUnionGeneration_Options_PropertyIdentifier).Value,
            CreatorAccessibility = unionTagAttr.GetNamedArgument<CreatorAccessibility>(Literals.CreatorAccessibility_PropertyIdentifier).Value,
        }, warnings);

        static string GetDefaultGeneratedUnionTypeIdentifier(ITypeSymbol unionTagType)
        {
            var enumTypeName = unionTagType.Name;
            if (enumTypeName.EndsWith("Tag") && enumTypeName.Length > 3)
                return enumTypeName[..^3];
            if (enumTypeName.EndsWith("Kind") && enumTypeName.Length > 4)
                return enumTypeName[..^4];
            return $"{enumTypeName}Union";
        }
    }


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
            CreatorAccessibility = CreatorAccessibility,
            GenerationOptions = GenerationOptions,
        };

        Variant BuildData(TagVariantData data, out int objCount, out bool containsUnmanaged)
        {
            objCount = 0;
            var unmanagedCount = 0;
            var fields = new List<VariantField>(data.Fields.Count);
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
                CreatorAccessibility = data.CreatorAccessibility,
            };
        }
    }
}
