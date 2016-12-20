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
    using System.Text;
    using System.Threading.Tasks;
    using QuickGraph;
    using QuickGraph.Algorithms;

    public static class GraphExtensions
    {
        public static HashSet<TVertex> GetVerticesNotInCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
            where TVertex : class
        {
            IDictionary<TVertex, int> components;
            inputGraph.StronglyConnectedComponents(out components);

            var grouped = components.GroupBy(x => x.Value, x => x.Key).ToArray();

            // All individual strongly connected vertices without self edges.
            var result =
                new HashSet<TVertex>(
                    grouped.Where(x => x.Count() == 1).SelectMany(x => x).Where(v => !inputGraph.InEdges(v).Any(e => e.IsSelfEdge<TVertex, Edge<TVertex>>())));

            return result;
        }

        public static HashSet<TVertex> GetCycles<TVertex>(this BidirectionalGraph<TVertex, Edge<TVertex>> inputGraph)
           where TVertex : class
        {
            IDictionary<TVertex, int> components;
            inputGraph.StronglyConnectedComponents(out components);

            var grouped = components.GroupBy(x => x.Value, x => x.Key).ToArray();

            var stronglyConnected = new HashSet<TVertex>(grouped.Where(x => x.Count() > 1).SelectMany(x => x));

            // All individual strongly connected vertices with self edges (i.e. single vertex cycles).
            var result = new HashSet<TVertex>(
                    grouped.Where(x => x.Count() == 1).SelectMany(x => x).Where(v => inputGraph.InEdges(v).Any(e => e.IsSelfEdge<TVertex, Edge<TVertex>>())));

            foreach (var stronglyConnectedComponent in stronglyConnected)
            {
                   
            }

            return result;
        }
    }
}
