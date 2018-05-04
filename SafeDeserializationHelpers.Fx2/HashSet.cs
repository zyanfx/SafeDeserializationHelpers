namespace System.Collections.Generic
{
    using System;
    using System.Collections;

    /// <summary>
    /// A .NET 2.0 polyfill for the HashSet{T} class.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    internal class HashSet<T> : ICollection<T>
    {
        private readonly Dictionary<T, object> dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">Item comparer.</param>
        public HashSet(IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            dict = new Dictionary<T, object>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class.
        /// </summary>
        /// <param name="items">Items to add.</param>
        /// <param name="comparer">Item comparer.</param>
        public HashSet(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
            : this(comparer)
        {
            if (items == null)
            {
                return;
            }

            foreach (T item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Gets the number of items contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of items contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count => dict.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the set.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            dict[item] = null;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            dict.Clear();
        }

        /// <summary>
        /// Checks if the set contains an item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True, if the set contains an item.</returns>
        public bool Contains(T item)
        {
            return dict.ContainsKey(item);
        }

        /// <summary>
        /// Copies the items of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the items copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of items in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type T cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0 || arrayIndex >= array.Length || arrayIndex >= Count)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            dict.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item) => dict.Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => dict.Keys.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}