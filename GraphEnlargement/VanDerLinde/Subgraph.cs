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
    using Combinatorics.Collections;
    using MoreLinq;
    using QuickGraph;
    using QuickGraph.Algorithms;

    /// <summary>
    /// The subgraph graph expansion algorithm from:
    /// J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    /// </summary>
    public class Subgraph : IGraphEnlargementAlgorithm
    {
        private readonly IGraphEnlargementAlgorithm subAlgorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subgraph"/> class.
        /// </summary>
        /// <param name="subAlgorithm">The sub algorithm to run.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="subAlgorithm"/> is null.</exception>
        public Subgraph(IGraphEnlargementAlgorithm subAlgorithm)
        {
            if (subAlgorithm == null)
            {
                throw new ArgumentNullException(nameof(subAlgorithm));
            }

            this.subAlgorithm = subAlgorithm;
        }

        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class
        {
            var result = inputGraph.Clone();
            var subGraph = inputGraph.Clone();

            var cycles = result.GetCycles();

            var nonRepeatedCycles = new List<IList<TVertex[]>>();
            for (int i = 1; i <= cycles.Count; i++)
            {
                var combinations = new Combinations<TVertex[]>(cycles, i);
                bool any = false;
                foreach (var combination in combinations)
                {
                    var set = new HashSet<TVertex>();
                    bool success = true;
                    foreach (var vertex in combination.SelectMany(x => x))
                    {
                        if (!set.Add(vertex))
                        {
                            success = false;
                            break;
                        }
                    }

                    if (success)
                    {
                        any = true;
                        nonRepeatedCycles.Add(combination);
                    }
                }

                if (!any)
                {
                    break;
                }
            }

            // Reverse needed to match behaviour of van der Linde.
            nonRepeatedCycles.Reverse();
            var longest = nonRepeatedCycles.MaxBy(x => x.SelectMany(y => y).Count());

            foreach (var cycle in longest)
            {
                foreach (var vertex in cycle)
                {
                    subGraph.RemoveVertex(vertex);
                }
            }

            foreach (var vertex in subGraph.Vertices)
            {
                result.RemoveVertex(vertex);
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
