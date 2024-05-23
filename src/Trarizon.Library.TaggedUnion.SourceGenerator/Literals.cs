using Microsoft.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator;
internal static class Literals
{
    public const string Namespace = "Trarizon.Library.TaggedUnion";
    public const string Category = $"{Namespace}.SourceGenerator";


    public const string UnionTagAttribute_TypeName = $"{Namespace}.Attributes.UnionTagAttribute";
    public const string TagVariantAttribute_TypeName = $"{Namespace}.Attributes.TagVariantAttribute";

    public const int UnionTagAttribute_UnionTypeName_CtorIndex = 0;
    public const string ITaggedUnionGeneration_Options_PropertyIdentifier = "Options";
    public const string CreatorAccessibility_PropertyIdentifier = "CreatorAccessibility";

    public const string IDisposable_TypeName = $"System.IDisposable";
    public const string IEquatable_TypeName = $"System.IEquatable";
    public const string Func_TypeName = $"System.Func";
    public const string IEqualityOperators_TypeName = $"System.Numerics.IEqualityOperators";
    public const string HashCode_TypeName = $"System.HashCode";
    public const string HashCode_Combine_MethodName = $"{HashCode_TypeName}.Combine";
    public const string EqualityComparer_TypeName = $"System.Collections.Generic.EqualityComparer";

    private const string Unsafe_TypeName = "System.Runtime.CompilerServices.Unsafe";
    private const string Unsafe_As_MethodIdentifier = "As";
    public const string Unsafe_As_MethodName = $"{Unsafe_TypeName}.{Unsafe_As_MethodIdentifier}";
    public const string Unsafe_SkipInit_MethodName = $"{Unsafe_TypeName}.SkipInit";

    private const string InteropServices_NamespaceName = "System.Runtime.InteropServices";
    public const string FieldOffsetAttribute_TypeName = $"{InteropServices_NamespaceName}.FieldOffsetAttribute";
    public const string StructLayoutAttribute_TypeName = $"{InteropServices_NamespaceName}.StructLayoutAttribute";
    private const string LayoutKind_TypeName = $"{InteropServices_NamespaceName}.LayoutKind";
    public const string LayoutKind_Explicit_FieldName = $"{LayoutKind_TypeName}.Explicit";

    private const string CodeAnalysis_NamespaceName = "System.Diagnostics.CodeAnalysis";
    public const string UnscopedRefAttribute_TypeName = $"{CodeAnalysis_NamespaceName}.UnscopedRefAttribute";



    public static class DiagnosticDescriptors
    {
        /// <summary>
        /// params: 
        /// - identifier(s) display string
        /// </summary>
        public static readonly DiagnosticDescriptor InvalidIdentifier = new(
            "TRATU0001",
            nameof(InvalidIdentifier),
            "Invalid identifier(s): {0}",
            Category,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor VariantFieldNameRepeat = new(
            $"TRATU0002",
            nameof(VariantFieldNameRepeat),
            "Variant field name repeat",
            Category,
            DiagnosticSeverity.Error,
            true);

        public static readonly DiagnosticDescriptor NoZeroVariantField = new(
            $"TRATU0003",
            nameof(NoZeroVariantField),
            "Do not attach field on zero, as user always can use default(T) to get this value",
            Category,
            DiagnosticSeverity.Warning,
            true);

    }
}
