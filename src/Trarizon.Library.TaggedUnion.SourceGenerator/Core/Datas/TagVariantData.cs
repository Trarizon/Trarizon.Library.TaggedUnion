using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Mirror;
using Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core.Datas;
internal sealed record TagVariantData(IFieldSymbol EnumField, IReadOnlyCollection<(ITypeSymbol Type, string Identifier)> Fields)
{
    public CreatorAccessibility CreatorAccessibility { get; init; }

    public TagVariantData(IFieldSymbol EnumField) : this(EnumField, []) { }

    public static TagVariantData? FromAttribute(AttributeData attribute, IFieldSymbol enumField)
    {
        var ctorArgs = attribute.ConstructorArguments;
        if (attribute.AttributeClass is not { } attr)
            return null;
        var typeArguments = attr.TypeArguments;

        if (typeArguments.Length == 0) {
            switch (ctorArgs) {
                // (params Type[] types)
                case [{ Kind: TypedConstantKind.Array } arg0]
                when ((IArrayTypeSymbol)arg0.Type!).ElementType.ToDisplayString() is "System.Type":
                    return new TagVariantData(
                        enumField,
                        arg0.Values
                            .Select((tc, i) => ((ITypeSymbol)tc.Value!, DefaultVariantFieldIdentifier(i)))
                            .ToImmutableArray()) {
                        CreatorAccessibility = attribute.GetNamedArgument<CreatorAccessibility>(Literals.CreatorAccessibility_PropertyIdentifier).Value,
                    };
                // (Type[], string?[]?)
                case [{ Kind: TypedConstantKind.Array } arg0, { Kind: TypedConstantKind.Array } arg1]
                when ((IArrayTypeSymbol)arg0.Type!).ElementType.ToDisplayString() is "System.Type" &&
                    ((IArrayTypeSymbol)arg1.Type!).ElementType.SpecialType is SpecialType.System_String:
                    return new TagVariantData(
                        enumField,
                        arg0.Values.Zip(
                            arg1.Values.Select((tc, i) => tc.IsNull ? DefaultVariantFieldIdentifier(i) : (string)tc.Value!),
                            (tc0, v1) => ((ITypeSymbol)tc0.Value!, v1))
                            .ToImmutableArray()) {
                        CreatorAccessibility = attribute.GetNamedArgument<CreatorAccessibility>(Literals.CreatorAccessibility_PropertyIdentifier).Value,
                    };
                // (Type, string, ...)
                case { } when IsAlternatingTypeStringParameters(ctorArgs):
                    var builder = ImmutableArray.CreateBuilder<(ITypeSymbol, string)>(ctorArgs.Length / 2);
                    for (int i = 0; i < ctorArgs.Length; i += 2) {
                        var t = ctorArgs[i];
                        var s = ctorArgs[i + 1];
                        builder.Add(((ITypeSymbol)t.Value!, s.IsNull ? DefaultVariantFieldIdentifier(i / 2) : (string)s.Value!));
                    }
                    return new TagVariantData(
                        enumField,
                        builder.ToImmutable()) {
                        CreatorAccessibility = attribute.GetNamedArgument<CreatorAccessibility>(Literals.CreatorAccessibility_PropertyIdentifier).Value,
                    };
                default:
                    return null;
            }
        }
        else {
            if (ctorArgs.Length != typeArguments.Length)
                return null;

            var ctorArgStrs = ctorArgs.Select((arg, index) => (string?)arg.Value ?? DefaultVariantFieldIdentifier(index));

            return new TagVariantData(
                enumField,
                typeArguments
                    .Zip(ctorArgStrs, (type, arg) => (type, arg))
                    .ToImmutableArray()) {
                CreatorAccessibility = attribute.GetNamedArgument<CreatorAccessibility>(Literals.CreatorAccessibility_PropertyIdentifier).Value,
            };
        }

        static string DefaultVariantFieldIdentifier(int index) => $"Item{index + 1}";

        static bool IsAlternatingTypeStringParameters(ImmutableArray<TypedConstant> args)
        {
            if (args.Length % 2 != 0)
                return false;

            for (int i = 0; i < args.Length; i += 2) {
                if (args[i].Kind is TypedConstantKind.Type && args[i + 1].Type?.SpecialType is SpecialType.System_String)
                    continue;
                else
                    return false;
            }
            return true;
        }
    }
}
