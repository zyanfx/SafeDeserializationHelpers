namespace System.Linq
{
    using System.Collections.Generic;

    /// <summary>
    /// A .NET 2.0 polyfill for the some of LINQ operators.
    /// </summary>
    internal static class Enumerable
    {
        /// <summary>
        /// Returns true if all items match the predicate.
        /// </summary>
        /// <typeparam name="TSource">The source item type.</typeparam>
        /// <param name="source">The source items.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>True if all items match the predicate, otherwise, false.</returns>
        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (var element in source)
            {
                if (!predicate(element))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Transforms the given sequence.
        /// </summary>
        /// <typeparam name="TSource">The source item type.</typeparam>
        /// <typeparam name="TResult">The result item type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="selector">The selector function</param>
        /// <returns>The transformed sequence.</returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            foreach (var item in source)
            {
                yield return selector(item);
            }
        }

        /// <summary>
        /// Converts the sequence to an array.
        /// </summary>
        /// <typeparam name="TResult">The result item type.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>The array.</returns>
        public static TResult[] ToArray<TResult>(this IEnumerable<TResult> source)
        {
            return new List<TResult>(source).ToArray();
        }
    }
}