//-----------------------------------------------------------------------
// <copyright file="GraphBuilder.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;

    /// <summary>
    /// Helper class to build directed graphs from input data.
    /// </summary>
    public class GraphBuilder
    {
        /// <summary>
        /// Builds a bi-directional graph from the specified input.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="input">The input vertices.</param>
        /// <param name="keySelector">The edge key selector.</param>
        /// <param name="targetSelector">The edge target selector.</param>
        /// <returns>A bi-directional graph from the specified input.</returns>
        public static BidirectionalGraph<TVertex, Edge<TVertex>> Build<TVertex, TKey>(IEnumerable<TVertex> input, Func<TVertex, TKey> keySelector, Func<TVertex, TKey> targetSelector)
        {
            var vertices = input as TVertex[] ?? input.ToArray();
            var map = vertices.GroupBy(targetSelector).ToDictionary(x => x.Key, x => x.ToArray());
            return vertices.ToBidirectionalGraph(
                x =>
                    {
                        TVertex[] targets;
                        if (map.TryGetValue(keySelector(x), out targets))
                        {
                            return targets.Select(y => new Edge<TVertex>(x, y));
                        }

                        return Enumerable.Empty<Edge<TVertex>>();
                    });
        }
    }
}
