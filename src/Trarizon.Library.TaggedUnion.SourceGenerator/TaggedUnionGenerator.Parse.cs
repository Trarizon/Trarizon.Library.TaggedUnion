using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Datas;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Mirror;
using Trarizon.Library.TaggedUnion.SourceGenerator.Helpers;
using Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;

namespace Trarizon.Library.TaggedUnion.SourceGenerator;
partial class TaggedUnionGenerator
{
    private static ParseResult<TaggedUnionData> ParseUnionTag(GeneratorAttributeSyntaxContext context, CancellationToken token)
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

                var tv = TagVariantData.From(attr, field);
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
            var zero = variants.FirstOrDefault(v => ValidationHelpers.IsZeroEnumValue(v.EnumField) && v.Fields.Length > 0);
            if (zero is not null) {
                warnings.Add(new DiagnosticData(
                    Literals.DiagnosticDescriptors.NoZeroVariantField,
                    zero.EnumField.DeclaringSyntaxReferences[0]));
            }
        }

        token.ThrowIfCancellationRequested();

        var options = unionTagAttr.GetNamedArgument<TaggedUnionGenerationOptions>(Literals.ITaggedUnionGeneration_Options_PropertyIdentifier).Value;

        return new(new TaggedUnionData(tagTypeSymbol, tagTypeSyntax, unionTypeIdentifier, options, variants), warnings);


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
}
