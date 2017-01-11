//-----------------------------------------------------------------------
// <copyright file="FirstPass.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement.Sanders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Configuration;
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;
    using QuickGraph.Algorithms;

    /// <summary>
    /// The first pass graph expansion algorith from:
    /// I. Sanders. Cooperating to buy shoes: An application of picking cycles in directed graphs.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists (Theme: "A Connected Society"), pages 8-16, East London, 2013.
    /// </summary>
    public class FirstPass : IGraphEnlargementAlgorithm
    {
        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex
        {
            var result = inputGraph.Clone();

            var disconnected = new Stack<TVertex>(result.Vertices.Where(x => result.IsInEdgesEmpty(x) && result.IsOutEdgesEmpty(x)));
            var noInEdge = new Stack<TVertex>(result.Vertices.Where(x => result.IsInEdgesEmpty(x) && !result.IsOutEdgesEmpty(x)));
            var noOutEdge = new Stack<TVertex>(result.Vertices.Where(x => !result.IsInEdgesEmpty(x) && result.IsOutEdgesEmpty(x)));
            var use = new Queue<TVertex>(result.Vertices.Where(x => !result.IsInEdgesEmpty(x) && !result.IsOutEdgesEmpty(x)).Reverse());

            while (disconnected.Count > 0 || noInEdge.Count > 0 || noOutEdge.Count > 0)
            {
                var first = noOutEdge.Count > 0 ? noOutEdge.Pop() : null;
                var last = noInEdge.Count > 0 ? noInEdge.Pop() : null;
                var mid = disconnected.Count > 0 ? disconnected.Pop() : null;

                if (mid != null)
                {
                    if (first == null)
                    {
                        first = use.Dequeue();
                        use.Enqueue(first);
                    }

                    if (last == null)
                    {
                        last = use.Dequeue();
                        use.Enqueue(last);
                    }

                    var dummy1 = vertexFactory("Dummy 1", first, mid);
                    var dummy2 = vertexFactory("Dummy 2", mid, last);
                    result.AddVertex(dummy1);
                    result.AddVertex(dummy2);

                    result.AddEdge(new Edge<TVertex>(first, dummy1));
                    result.AddEdge(new Edge<TVertex>(dummy1, mid));
                    result.AddEdge(new Edge<TVertex>(mid, dummy2));
                    result.AddEdge(new Edge<TVertex>(dummy2, last));
                }
                else
                {
                    if (first == null)
                    {
                        first = use.Dequeue();
                        use.Enqueue(first);
                    }

                    if (last == null)
                    {
                        last = use.Dequeue();
                        use.Enqueue(last);
                    }

                    var dummy3 = vertexFactory("Dummy 3", first, last);
                    result.AddVertex(dummy3);

                    result.AddEdge(new Edge<TVertex>(first, dummy3));
                    result.AddEdge(new Edge<TVertex>(dummy3, last));
                }
            }

            return result;
        }
    }
}
