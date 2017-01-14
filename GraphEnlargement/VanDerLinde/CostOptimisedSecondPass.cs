//-----------------------------------------------------------------------
// <copyright file="CostOptimisedSecondPass.cs" company="Richard Smith">
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

    /// <summary>
    /// The second pass cost optimised graph expansion algorithm from:
    /// J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    /// </summary>
    public class CostOptimisedSecondPass : IGraphEnlargementAlgorithm
    {
        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex
        {
            var result = inputGraph.Clone();

            HashSet<TVertex> noCycles = result.GetVerticesNotInCycles();

            while (noCycles.Count > 0)
            {
                var x = noCycles.First();

                var b = result.OutEdges(x).First().Target;
                while (noCycles.Contains(b))
                {
                    b = result.OutEdges(b).First().Target;
                }

                var e = result.InEdges(x).First().Source;
                while (noCycles.Contains(e))
                {
                    e = result.InEdges(e).First().Source;
                }

                var dummy = vertexFactory("Dummy", b, e);
                result.AddVertex(dummy);

                result.AddEdge(new Edge<TVertex>(b, dummy));
                result.AddEdge(new Edge<TVertex>(dummy, e));

                noCycles = result.GetVerticesNotInCycles();
            }

            return result;
        }
    }
}
