using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Trarizon.Library.TaggedUnion.SourceGenerator.Core.Datas;

namespace Trarizon.Library.TaggedUnion.SourceGenerator;
[Generator(LanguageNames.CSharp)]
internal sealed partial class TaggedUnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var res = context.SyntaxProvider.ForAttributeWithMetadataName(
            Literals.UnionTagAttribute_TypeName,
            (node, token) => node is EnumDeclarationSyntax,
            UnionTagData.Parse)
            .Select((res, token) => res.Select(v => v.Build()));

        context.RegisterSourceOutput(res, (context, res) =>
        {
            foreach (var diag in res.Diagnostics) {
                context.ReportDiagnostic(diag.ToDiagnostic());
            }
            if (res.HasValue) {
                var union = res.Value;
                var hintName = $"{union.TypeNameFullQualified.Replace('<', '{').Replace('>', '}')}.g.cs";
                var source = union.Emit();
#if DEBUG
                System.Console.WriteLine(source);
#endif
                context.AddSource(hintName, source);
            }
        });
    }
}
