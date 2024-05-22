using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
internal struct ParseResult<T>
{
    private readonly Optional<T> _value;
    private List<DiagnosticData>? _diagnostics;

    public readonly T Value => _value.Value;

    public readonly bool HasValue => _value.HasValue;

    public readonly IEnumerable<DiagnosticData> Diagnostics => _diagnostics ?? [];

    public ParseResult(T value) => _value = value;

    public ParseResult(DiagnosticData diagnostic)
    {
        _diagnostics = [diagnostic];
    }

    private ParseResult(List<DiagnosticData> diagnostics)
    {
        _diagnostics = diagnostics;
    }

    internal ParseResult(Optional<T> value, List<DiagnosticData>? diagnostics)
    {
        _value = value;
        _diagnostics = diagnostics;
    }

    public static implicit operator ParseResult<T>(T value) => new ParseResult<T>(value);
    public static implicit operator ParseResult<T>(DiagnosticData diagnostic) => new ParseResult<T>(diagnostic);
    public static implicit operator ParseResult<T>(List<DiagnosticData> diagnostics) => new ParseResult<T>(diagnostics);

    public readonly ParseResult<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        if (HasValue) {
            return new(selector(_value.Value), _diagnostics);
        }
        else {
            return new(default, _diagnostics);
        }
    }
}
