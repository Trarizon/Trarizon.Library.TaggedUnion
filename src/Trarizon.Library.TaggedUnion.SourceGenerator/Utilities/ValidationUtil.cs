using System;
using System.Diagnostics.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
internal static class ValidationUtil
{
    public static bool IsValidIdentifier([NotNullWhen(true)] string? identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;

        if (identifier![0] is not ('_' or (>= 'a' and <= 'z') or (>= 'A' and <= 'Z')))
            return false;

        foreach (var c in identifier.AsSpan(1)) {
            if (c is not ('_' or (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9')))
                return false;
        }

        return true;
    }
}
