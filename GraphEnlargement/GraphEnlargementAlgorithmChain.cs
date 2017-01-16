//-----------------------------------------------------------------------
// <copyright file="GraphEnlargementAlgorithmChain.cs" company="Richard Smith">
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
    /// A chain of <see cref="IGraphEnlargementAlgorithm"/>s.
    /// </summary>
    public class GraphEnlargementAlgorithmChain : IGraphEnlargementAlgorithm
    {
        private readonly IGraphEnlargementAlgorithm[] algorithms;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphEnlargementAlgorithmChain"/> class.
        /// </summary>
        /// <param name="algorithms">The algorithms.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="algorithms"/> are null.</exception>
        public GraphEnlargementAlgorithmChain(IEnumerable<IGraphEnlargementAlgorithm> algorithms)
        {
            if (algorithms == null)
            {
                throw new ArgumentNullException(nameof(algorithms));
            }

            this.algorithms = algorithms.ToArray();
        }

        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex
        {
            var result = inputGraph.Clone();

            foreach (var algorithm in this.algorithms)
            {
                result = algorithm.Apply(result, vertexFactory);
            }

            return result;
        }
    }
}
