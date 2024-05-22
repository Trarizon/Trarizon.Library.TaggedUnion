using Microsoft.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Helpers;
internal static class ValidationHelpers
{
    public static bool IsZeroEnumValue(IFieldSymbol field)
    {
        return field.ConstantValue switch {
            byte b => b == 0,
            sbyte sb => sb == 0,
            short s => s == 0,
            ushort us => us == 0,
            int i => i == 0,
            uint ui => ui == 0,
            long l => l == 0,
            ulong ul => ul == 0,
            _ => false,
        };
    }
}
