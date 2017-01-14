//-----------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargementTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraphEnlargement;
    using QuickGraph;

    /// <summary>
    /// Base class for unit tests.
    /// </summary>
    public abstract class TestBase
    {
        public static List<MismatchedShoePerson> GenerateInput(int count, Random random, bool excludeSelfCycles)
        {
            var result = new List<MismatchedShoePerson>();

            for (int i = 0; i < count; i++)
            {
                var item = new MismatchedShoePerson() { Name = $"Node {i + 1}", LeftSize = random.Next(1, 10), RightSize = random.Next(4, 13) };
                if (excludeSelfCycles && item.LeftSize == item.RightSize)
                {
                    i--;
                    continue;
                }

                result.Add(item);
            }

            return result;
        }

        public static string GetGraphInformation(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
        {
            return $"{graph.GetDescription()}{Environment.NewLine}Edges Valid: {(ValidateEdges(graph) ? "Yes" : "No")}{Environment.NewLine}Cycles Valid: {(ValidateCycles(graph) ? "Yes" : "No")}{Environment.NewLine}";
        }

        public static string GetGraphInformation(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph, Stopwatch stopwatch)
        {
            return $"{GetGraphInformation(graph)}Time: {stopwatch.ElapsedMilliseconds}ms{Environment.NewLine}";
        }

        public static bool ValidateCycles(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
        {
            return graph.GetVerticesNotInCycles().Count == 0;
        }

        public static bool ValidateEdges(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source.LeftSize != edge.Target.RightSize)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
