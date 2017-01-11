//-----------------------------------------------------------------------
// <copyright file="Permutation.cs" company="Richard Smith">
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
    /// The permutation graph expansion algorithm from:
    /// J. van der Linde, I. Sanders. Enlarging Directed Graphs To Ensure All Nodes Are Contained In Cycles.
    /// In Proceedings of the South African Institute of Computer Scientists and Information Technologists, 2015.
    /// </summary>
    public class Permutation : IGraphEnlargementAlgorithm
    {
        private readonly bool removeRedunantVertices;

        /// <summary>
        /// Initializes a new instance of the <see cref="Permutation"/> class.
        /// </summary>
        public Permutation()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permutation"/> class.
        /// </summary>
        /// <param name="removeRedunantVertices">if set to <c>true</c> remove redunant vertices from the output.</param>
        public Permutation(bool removeRedunantVertices)
        {
            this.removeRedunantVertices = removeRedunantVertices;
        }

        /// <inheritdoc/>
        public BidirectionalGraph<TVertex, Edge<TVertex>> Apply<TVertex>(BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph, Func<string, TVertex, TVertex, TVertex> vertexFactory)
            where TVertex : class, IGraphEnlargementVertex
        {
            var verticesMap = inputGraph.Vertices.Select((x, i) => new { Vertex = x, Index = i }).ToDictionary(x => x.Index, x => x.Vertex);
            var reverseVerticesMap = verticesMap.ToDictionary(x => x.Value, x => x.Key);

            var matrix = new int[inputGraph.VertexCount + 1, inputGraph.VertexCount + 1];

            foreach (var edge in inputGraph.Edges)
            {
                var x = reverseVerticesMap[edge.Target];
                var y = reverseVerticesMap[edge.Source];

                matrix[x, y]++;
                matrix[inputGraph.VertexCount, y]++;
                matrix[x, inputGraph.VertexCount]++;
            }

            var result = inputGraph.Clone();

            // Remove redundant
            for (int y = matrix.GetLength(1) - 2; y >= 0; y--)
            {
                if (matrix[matrix.GetLength(0) - 1, y] > 1)
                {
                    for (int x = matrix.GetLength(0) - 2; x >= 0 && matrix[matrix.GetLength(0) - 1, y] > 1; x--)
                    {
                        if (matrix[x, y] == 1)
                        {
                            matrix[x, y] = 0;
                            matrix[matrix.GetLength(0) - 1, y]--;
                            matrix[x, matrix.GetLength(1) - 1]--;

                            RemoveEdge(verticesMap, x, y, result);
                        }
                    }
                }
            }

            for (int x = matrix.GetLength(0) - 2; x >= 0; x--)
            {
                if (matrix[x, matrix.GetLength(1) - 1] > 1)
                {
                    for (int y = matrix.GetLength(1) - 2; y >= 0 && matrix[x, matrix.GetLength(1) - 1] > 1; y--)
                    {
                        if (matrix[x, y] == 1)
                        {
                            matrix[x, y] = 0;
                            matrix[matrix.GetLength(0) - 1, y]--;
                            matrix[x, matrix.GetLength(1) - 1]--;

                            RemoveEdge(verticesMap, x, y, result);
                        }
                    }
                }
            }

            // Expand
            var zeroColumns = new Stack<int>();
            var zeroRows = new Stack<int>();
            int dummyNodeCount = 0;

            for (int x = 0; x < matrix.GetLength(0) - 1; x++)
            {
                if (matrix[x, matrix.GetLength(1) - 1] == 0)
                {
                    zeroColumns.Push(x);
                    dummyNodeCount++;
                }
            }

            for (int y = matrix.GetLength(1) - 2; y >= 0; y--)
            {
                if (matrix[matrix.GetLength(0) - 1, y] == 0)
                {
                    zeroRows.Push(y);
                }
            }

            for (int i = 0; i < dummyNodeCount; i++)
            {
                var source = verticesMap[zeroRows.Pop()];
                var target = verticesMap[zeroColumns.Pop()];

                var dummy = vertexFactory("Dummy", source, target);
                result.AddVertex(dummy);

                result.AddEdge(new Edge<TVertex>(source, dummy));
                result.AddEdge(new Edge<TVertex>(dummy, target));
            }

            return this.removeRedunantVertices ? new RedundantNodeRemoval().Apply(result, vertexFactory) : result;
        }

        private static void RemoveEdge<TVertex>(Dictionary<int, TVertex> verticesMap, int x, int y, BidirectionalGraph<TVertex, Edge<TVertex>> graph)
            where TVertex : class
        {
            var source = verticesMap[y];
            var target = verticesMap[x];
            Edge<TVertex> edge;
            if (graph.TryGetEdge(source, target, out edge))
            {
                graph.RemoveEdge(edge);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
