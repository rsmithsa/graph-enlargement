//-----------------------------------------------------------------------
// <copyright file="RevisedSubGraph.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement.Smith
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;

    public class RevisedSubGraph : IGraphEnlargementAlgorithm
    {
        private readonly IGraphEnlargementAlgorithm subAlgorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevisedSubGraph"/> class.
        /// </summary>
        /// <param name="subAlgorithm">The sub algorithm to run.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="subAlgorithm"/> is null.</exception>
        public RevisedSubGraph(IGraphEnlargementAlgorithm subAlgorithm)
        {
            if (subAlgorithm == null)
            {
                throw new ArgumentNullException(nameof(subAlgorithm));
            }

            this.subAlgorithm = subAlgorithm;
        }

        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex
        {
            var result = inputGraph.Clone();
            var subGraph = inputGraph.Clone();

            var notInCycles = result.GetVerticesNotInCycles();

            foreach (var vertex in notInCycles)
            {
                result.RemoveVertex(vertex);
            }

            foreach (var vertex in result.Vertices)
            {
                subGraph.RemoveVertex(vertex);
            }

            subGraph = this.subAlgorithm.Apply(subGraph, vertexFactory);

            foreach (var vertex in subGraph.Vertices)
            {
                result.AddVertex(vertex);
            }

            foreach (var edge in subGraph.Edges)
            {
                result.AddEdge(edge);
            }

            return result;
        }
    }
}
