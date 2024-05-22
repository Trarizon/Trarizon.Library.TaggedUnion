using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
public static class Optional
{
    public static Optional<T> Of<T>(T value) => new(value);

    /// <summary>
    /// In fact just use <c>default</c> is ok
    /// </summary>
    public static Optional<T> None<T>() => default;

    public static Optional<T> FromNullable<T>(T? value) where T : struct
        => value.HasValue ? value.GetValueOrDefault()! : default;

    public static T? ToNullable<T>(this Optional<T> value) where T : struct
        => value.HasValue ? value.Value : default;

    public static Optional<T> Unwrap<T>(this Optional<Optional<T>> optional)
        => optional.HasValue ? optional.Value : default;

    public static Optional<TResult> Select<T, TResult>(this Optional<T> optional, Func<T, TResult> selector)
        => optional.HasValue ? new(selector(optional.Value)) : default;

    public static Optional<TResult> SelectWrapped<T, TResult>(this Optional<T> optional, Func<T, Optional<TResult>> selector)
        => optional.HasValue ? selector(optional.Value) : default;

    public static bool TryGetValue<T>(this Optional<T> optional, [MaybeNullWhen(false)] out T value)
    {
        value = optional.Value;
        return optional.HasValue;
    }
}
