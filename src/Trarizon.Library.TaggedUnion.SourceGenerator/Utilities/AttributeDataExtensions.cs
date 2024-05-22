using Microsoft.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
internal static class AttributeDataExtensions
{
    public static Optional<T> GetConstructorArgument<T>(this AttributeData attribute, int index)
    {
        if (attribute.ConstructorArguments is var args && index >= 0 && index < args.Length)
            return (T)args[index].Value!;
        else
            return default;
    }

    public static Optional<T> GetNamedArgument<T>(this AttributeData attribute, string name)
    {
        if (attribute.NamedArguments.TryFirst(kv => kv.Key == name, out var first))
            return (T)first.Value.Value!;
        else
            return default;
    }

}
