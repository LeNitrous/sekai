using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns whether a given enumerable is a subset of the other enumerable.
    /// </summary>
    public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b)
    {
        return !a.Except(b).Any();
    }
}
