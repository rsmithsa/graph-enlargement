//-----------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Various helper LINQ extension methods.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Drops the last element of a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <returns>An enumerable without the last element of the <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is null.</exception>
        public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    for (var val = enumerator.Current; enumerator.MoveNext(); val = enumerator.Current)
                    {
                        yield return val;
                    }
                }
            }
        }
    }
}
