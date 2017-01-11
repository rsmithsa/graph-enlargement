//-----------------------------------------------------------------------
// <copyright file="CostOptimisedFirstPass.cs" company="Richard Smith">
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
    /// The first pass cost optimised graph expansion algorithm from:
    /// J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    /// </summary>
    public class CostOptimisedFirstPass : IGraphEnlargementAlgorithm
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

            IDictionary<TVertex, int> components;
            inputGraph.StronglyConnectedComponents(out components);

            while (disconnected.Count > 0 || noInEdge.Count > 0 || noOutEdge.Count > 0)
            {
                var first = noOutEdge.Count > 0 ? noOutEdge.Pop() : null;
                var last = noInEdge.Count > 0 ? noInEdge.Pop() : null;
                var mid = disconnected.Count > 0 ? disconnected.Pop() : null;

                if (mid != null)
                {
                    if (first == null && last == null)
                    {
                        var dummyIsolated = vertexFactory("Dummy Isolated", mid, mid);
                        result.AddVertex(dummyIsolated);

                        result.AddEdge(new Edge<TVertex>(mid, dummyIsolated));
                        result.AddEdge(new Edge<TVertex>(dummyIsolated, mid));
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

                        var dummy1 = vertexFactory("Dummy 1", first, mid);
                        var dummy2 = vertexFactory("Dummy 2", mid, last);
                        result.AddVertex(dummy1);
                        result.AddVertex(dummy2);

                        result.AddEdge(new Edge<TVertex>(first, dummy1));
                        result.AddEdge(new Edge<TVertex>(dummy1, mid));
                        result.AddEdge(new Edge<TVertex>(mid, dummy2));
                        result.AddEdge(new Edge<TVertex>(dummy2, last));
                    }
                }
                else
                {
                    if (first == null)
                    {
                        if (last != null)
                        {
                            IEnumerable<Edge<TVertex>> outEdges;
                            if (result.TryGetOutEdges(last, out outEdges))
                            {
                                var subGraphs = new HashSet<int>(outEdges.Select(x => components[x.Source]));
                                if (subGraphs.Count == 1)
                                {
                                    var targetSubGraph = subGraphs.First();
                                    if (use.Any(x => components[x] == targetSubGraph))
                                    {
                                        do
                                        {
                                            first = use.Dequeue();
                                            use.Enqueue(first);
                                        }
                                        while (components[first] != targetSubGraph);
                                    }
                                }
                            }
                        }
                        else
                        {
                            first = use.Dequeue();
                            use.Enqueue(first);
                        }
                    }

                    if (last == null)
                    {
                        IEnumerable<Edge<TVertex>> inEdges;
                        if (result.TryGetInEdges(first, out inEdges))
                        {
                            var subGraphs = new HashSet<int>(inEdges.Select(x => components[x.Source]));
                            if (subGraphs.Count == 1)
                            {
                                var targetSubGraph = subGraphs.First();
                                if (use.Any(x => components[x] == targetSubGraph))
                                {
                                    do
                                    {
                                        last = use.Dequeue();
                                        use.Enqueue(last);
                                    }
                                    while (components[last] != targetSubGraph);
                                }
                            }
                        }

                        if (last == null)
                        {
                            last = use.Dequeue();
                            use.Enqueue(last);
                        }
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
