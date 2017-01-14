//-----------------------------------------------------------------------
// <copyright file="GraphExtensions.cs" company="Richard Smith">
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
    using QuickGraph.Algorithms;

    /// <summary>
    /// Various helper graph extension methods.
    /// </summary>
    public static class GraphExtensions
    {
        /// <summary>
        /// Gets the graph vertices not contained in cycles.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="inputGraph">The input graph.</param>
        /// <returns>The graph vertices not contained in cycles.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="inputGraph"/> is null.</exception>
        public static HashSet<TVertex> GetVerticesNotInCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
            where TVertex : class
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            IDictionary<TVertex, int> components;
            inputGraph.StronglyConnectedComponents(out components);

            var grouped = components.GroupBy(x => x.Value, x => x.Key).ToArray();

            // All individual strongly connected vertices without self edges.
            var result =
                new HashSet<TVertex>(
                    grouped.Where(x => x.Count() == 1).SelectMany(x => x).Where(v => !inputGraph.InEdges(v).Any(e => e.IsSelfEdge<TVertex, Edge<TVertex>>())));

            return result;
        }

        /// <summary>
        /// Gets the graph vertices contained in cycles.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="inputGraph">The input graph.</param>
        /// <returns>The graph vertices contained in cycles.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="inputGraph"/> is null.</exception>
        public static HashSet<TVertex> GetVerticesInCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
            where TVertex : class
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            var vertices = new HashSet<TVertex>(inputGraph.Vertices);

            vertices.ExceptWith(inputGraph.GetVerticesNotInCycles());

            return vertices;
        }

        /// <summary>
        /// Gets the cycles in the graph using Johnson's algorithm.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="inputGraph">The input graph.</param>
        /// <returns>The cycles in the graph.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="inputGraph"/> is null.</exception>
        public static IList<TVertex[]> GetCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
            where TVertex : class
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            var johnsons = new JohnsonsAlgorithm<TVertex>();
            johnsons.Process(inputGraph);
            return johnsons.Cycles;
        }

        /// <summary>
        /// Gets a string description of the graph.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="inputGraph">The input graph.</param>
        /// <returns>A string description of the graph.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="inputGraph"/> is null.</exception>
        public static string GetDescription<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            return $"(Vertices - {inputGraph.VertexCount}, Edges - {inputGraph.EdgeCount})";
        }
    }
}
