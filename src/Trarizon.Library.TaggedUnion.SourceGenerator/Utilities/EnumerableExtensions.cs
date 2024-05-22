using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
internal static class EnumerableExtensions
{
    #region On object

    public static IEnumerable<T> EnumerateByWhileNotNull<T>(this T? first, Func<T, T?> nextSelector)
    {
        while (first is not null) {
            yield return first;
            first = nextSelector(first);
        }
    }

    #endregion

    public static IEnumerable<T> DuplicatesBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        => source
        .GroupBy(keySelector)
        .Where(g => g.Count() > 1)
        .SelectMany(g => g);

    public static IEnumerable<(int Index, T Item)> Index<T>(this IEnumerable<T> source)
    {
        int i = 0;
        foreach (var item in source) {
            yield return (i, item);
            i++;
        }
    }

    public static IEnumerable<(int Index, T Item)> IndexBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
    {
        Dictionary<TKey, int> dict = new(comparer);
        foreach (var item in source) {
            var key = keySelector(item);
            if (dict.TryGetValue(key, out int val)) {
                yield return (val, item);
                dict[key] = val + 1;
            }
            else {
                yield return (0, item);
                dict.Add(key, 1);
            }
        }
    }

    public static bool IsDistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        HashSet<TKey> set = [];
        foreach (var item in source) {
            if (!set.Add(keySelector(item)))
                return false;
        }
        return true;
    }

    public static IEnumerable<T> OfNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        foreach (var item in source) {
            if(item is not null)
                yield return item;
        }
    }

    public static IEnumerable<T> OfTypeWhile<T>(this IEnumerable source)
    {
        foreach (var item in source) {
            if (item is T t)
                yield return t;
            else
                yield break;
        }
    }

    public static List<T>? ToListIfAny<T>(this IEnumerable<T> source)
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            return null;
        List<T> result = [enumerator.Current];

        while (enumerator.MoveNext()) {
            result.Add(enumerator.Current);
        }
        return result;
    }

    public static List<T>? ToListIfAny<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            return null;
        var current = enumerator.Current;
        if (!predicate(current))
            return null;

        List<T> result = [enumerator.Current];

        while (enumerator.MoveNext()) {
            if (!predicate(current))
                return null;
            result.Add(enumerator.Current);
        }
        return result;
    }

    public static bool TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T value)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext()) {
            var current = enumerator.Current;
            if (predicate(current)) {
                value = current;
                return true;
            }
        }
        value = default!;
        return false;
    }

    public static IEnumerable<TResult> WhereSelect<T, TResult>(this IEnumerable<T> source, Func<T, Optional<TResult>> filter)
    {
        foreach (var item in source) {
            var res = filter(item);
            if (res.TryGetValue(out var val))
                yield return val;
        }
    }
}
