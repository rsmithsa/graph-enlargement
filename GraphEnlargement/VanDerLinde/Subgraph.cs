//-----------------------------------------------------------------------
// <copyright file="Subgraph.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement.VanDerLinde
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;
    using QuickGraph.Algorithms;

    /// <summary>
    /// The subgraph graph expansion algorithm from:
    /// J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    /// </summary>
    public class Subgraph : IGraphEnlargementAlgorithm
    {
        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory) where TVertex : class
        {
            var result = inputGraph.Clone();

            IDictionary<TVertex, int> components;
            var c = result.StronglyConnectedComponents(out components);

            IDictionary<TVertex, int> weakComponents = new Dictionary<TVertex, int>(components);
            var w = result.WeaklyConnectedComponents(weakComponents);

            var grouped = components.GroupBy(x => x.Value, x => x.Key).ToArray();

            var cycles = new HashSet<TVertex>(grouped.Where(x => x.Count() > 1).SelectMany(x => x));
            var noCycles = result.GetVerticesNotInCycles();

            foreach (var vertex in cycles)
            {
                result.RemoveVertex(vertex);
            }

            return result;
        }
    }
}
