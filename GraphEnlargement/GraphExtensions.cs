//-----------------------------------------------------------------------
// <copyright file="GraphExtensions.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;
    using QuickGraph.Algorithms;

    public static class GraphExtensions
    {
        public static HashSet<TVertex> GetVerticesNotInCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
            where TVertex : class
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            IDictionary<TVertex, int> components;
            inputGraph.StronglyConnectedComponents(out components);

            var grouped = components.GroupBy(x => x.Value, x => x.Key).ToArray();

            // All individual strongly connected vertices without self edges.
            var result =
                new HashSet<TVertex>(
                    grouped.Where(x => x.Count() == 1).SelectMany(x => x).Where(v => !inputGraph.InEdges(v).Any(e => e.IsSelfEdge<TVertex, Edge<TVertex>>())));

            return result;
        }

        public static HashSet<TVertex> GetVerticesInCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph) where TVertex : class
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            var vertices = new HashSet<TVertex>(inputGraph.Vertices);

            vertices.ExceptWith(inputGraph.GetVerticesNotInCycles());

            return vertices;
        }

        public static IList<TVertex[]> GetCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
           where TVertex : class
        {
            if (inputGraph == null)
            {
                throw new ArgumentNullException(nameof(inputGraph));
            }

            var johnsons = new JohnsonsAlgorithm<TVertex>();
            johnsons.Process(inputGraph);
            return johnsons.Cycles;
        }
    }
}
