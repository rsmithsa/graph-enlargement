//-----------------------------------------------------------------------
// <copyright file="RedundantNodeRemoval.cs" company="Richard Smith">
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
    /// The redundant node removal algorithm from:
    /// J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    /// </summary>
    public class RedundantNodeRemoval : IGraphEnlargementAlgorithm
    {
        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex
        {
            var result = inputGraph.Clone();

            foreach (var vertex in inputGraph.Vertices)
            {
                if (vertex.GetKey().Equals(vertex.GetTarget()))
                {
                    var inEdges = result.InEdges(vertex).ToArray();
                    var outEdges = result.OutEdges(vertex).ToArray();

                    foreach (var inEdge in inEdges)
                    {
                        foreach (var outEdge in outEdges)
                        {
                            result.AddEdge(new Edge<TVertex>(inEdge.Source, outEdge.Target));
                        }
                    }

                    result.RemoveVertex(vertex);
                }
            }

            return result;
        }
    }
}
