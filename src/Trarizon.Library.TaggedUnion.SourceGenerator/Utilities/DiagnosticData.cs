using Microsoft.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
public sealed record DiagnosticData(DiagnosticDescriptor Descriptor, Location? Location, params object?[]? MessageArgs)
{
    public Diagnostic ToDiagnostic() => Diagnostic.Create(Descriptor, Location, MessageArgs);

    public DiagnosticData(DiagnosticDescriptor descriptor, SyntaxNode? syntax, params object?[]? messageArgs) :
        this(descriptor, syntax?.GetLocation(), messageArgs)
    { }

    public DiagnosticData(DiagnosticDescriptor descriptor, in SyntaxToken syntax, params object?[]? messageArgs) :
        this(descriptor, syntax.GetLocation(), messageArgs)
    { }

    public DiagnosticData(DiagnosticDescriptor descriptor, in SyntaxToken? syntax, params object?[]? messageArgs) :
        this(descriptor, syntax?.GetLocation(), messageArgs)
    { }

    public DiagnosticData(DiagnosticDescriptor descriptor, SyntaxReference? syntax, params object?[]? messageArgs) :
        this(descriptor, syntax?.GetSyntax().GetLocation(), messageArgs)
    { }
}
