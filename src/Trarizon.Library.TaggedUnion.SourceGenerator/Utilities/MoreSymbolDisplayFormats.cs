using Microsoft.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
public static class MoreSymbolDisplayFormats
{
    public static readonly SymbolDisplayFormat DefaultWithoutGeneric = SymbolDisplayFormat.CSharpErrorMessageFormat.WithGenericsOptions(SymbolDisplayGenericsOptions.None);
}
