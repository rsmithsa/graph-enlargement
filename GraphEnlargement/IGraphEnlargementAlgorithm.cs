//-----------------------------------------------------------------------
// <copyright file="IGraphEnlargementAlgorithm.cs" company="Richard Smith">
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
    /// Interface for a graph enlargement algorithm.
    /// </summary>
    public interface IGraphEnlargementAlgorithm
    {
        /// <summary>
        /// Applies the algorithm to the specified input graph.
        /// </summary>
        /// <typeparam name="TVertex">The type of the vertex.</typeparam>
        /// <param name="inputGraph">The input graph.</param>
        /// <param name="vertexFactory">The vertex factory function.</param>
        /// <returns>The resulting graph after applying the algorithm.</returns>
        BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex;
    }
}
