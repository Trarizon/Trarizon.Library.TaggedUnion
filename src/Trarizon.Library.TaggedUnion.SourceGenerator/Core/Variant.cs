using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Mirror;
using Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core;
internal unsafe sealed record Variant(
    string Name,
    IReadOnlyList<VariantField> Fields)
{
    public required int UnmanagedFieldCount { get; init; }

    public CreatorAccessibility CreatorAccessibility { get; init; }

    // Lazy Properties

    private string? __parameterListString;
    public string ParameterListString => __parameterListString ??= string.Join(", ", Fields.Select(field => $"{field.Type.ToFullQualifiedDisplayString()} {field.Name}"));

    private string? __outParameterListString;
    public string OutParameterListString => __outParameterListString ??= string.Join(", ", Fields.Select(field => $"out {field.Type.ToFullQualifiedDisplayString()} {field.Name}"));

    private Optional<string?> __unmanagedTupleTypeName;
    public string? UnmanagedTupleTypeName => __unmanagedTupleTypeName.HasValue
        ? __unmanagedTupleTypeName.Value
        : (__unmanagedTupleTypeName = UnmanagedFieldCount switch {
            0 => null,
            1 => Fields
                .First(vf => vf.UnionField.Kind is UnionFieldKind.Unmanaged)
                .Type.ToFullQualifiedDisplayString(),
            _ => $"({string.Join(", ", Fields.Where(vf => vf.UnionField.Kind is UnionFieldKind.Unmanaged).Select(vf => vf.Type.ToFullQualifiedDisplayString()))})",
        }).Value;

    private IntPtr __unmanagedTupleItemAccess;
    public unsafe string? UnmanagedTupleItemAccess(int tupleItemIndex)
    {
        if (UnmanagedFieldCount == 0)
            throw new InvalidOperationException("No unmanaged type in current variant.");

        if (__unmanagedTupleItemAccess == default) {
            __unmanagedTupleItemAccess = UnmanagedFieldCount switch {
                1 => (IntPtr)(delegate*<int, string?>)&GetSingleItemAccess,
                _ => (IntPtr)(delegate*<int, string?>)&GetMultipleItemAccess,
            };

            static string? GetSingleItemAccess(int index) => null;
            static string? GetMultipleItemAccess(int tupleItemIndex) => $".Item{tupleItemIndex}";
        }

        return ((delegate*<int, string?>)__unmanagedTupleItemAccess)(tupleItemIndex);
    }
}
