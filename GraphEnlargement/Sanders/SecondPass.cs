//-----------------------------------------------------------------------
// <copyright file="SecondPass.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement.Sanders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;

    /// <summary>
    /// The second pass graph expansion algorith from:
    /// I. Sanders. Cooperating to buy shoes: An application of picking cycles in directed graphs.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists (Theme: "A Connected Society"), pages 8-16, East London, 2013.
    /// </summary>
    public class SecondPass : IGraphEnlargementAlgorithm
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
