//-----------------------------------------------------------------------
// <copyright file="PerformanceTests.cs" company="Richard Smith">
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
    using GraphEnlargement.Sanders;
    using GraphEnlargement.Smith;
    using GraphEnlargement.VanDerLinde;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using QuickGraph;

    /// <summary>
    /// A set of performance tests for the various graph enlargement algorithms.
    /// </summary>
    [TestClass]
    public class PerformanceTests : TestBase
    {
        public const int Start = 16;
        public const int End = 512;
        public static readonly int[] Seeds = { 1434747710, 302596119, 269548474 };

        [TestMethod]
        public void TestSandersPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new FirstPass(), new SecondPass());
            }
        }

        [TestMethod]
        public void TestPermutationPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new Permutation());
            }
        }

        [TestMethod]
        public void TestPermutationSubgraphPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new Subgraph(new Permutation()));
            }
        }

        [TestMethod]
        public void TestPermutationRevisedSubgraphPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new RevisedSubgraph(new Permutation()));
            }
        }

        private static void ExecuteAlgorithms(Random random, params IGraphEnlargementAlgorithm[] algorithms)
        {
            for (int i = Start; i <= End; i *= 2)
            {
                BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph;
                do
                {
                    var input = GenerateInput(i, random, true);
                    graph = GraphBuilder.Build(input, x => x.LeftSize, x => x.RightSize);
                }
                while (graph.GetVerticesNotInCycles().Count == 0);

                Console.WriteLine($"Input: {GetGraphInformation(graph)}");

                try
                {
                    BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> result = graph;
                    var sw = Stopwatch.StartNew();

                    foreach (var algorithm in algorithms)
                    {
                        result = algorithm.Apply(
                            result,
                            (name, inVertex, outVertex) => new MismatchedShoePerson() { Name = name, LeftSize = outVertex.RightSize, RightSize = inVertex.LeftSize });
                    }

                    sw.Stop();

                    Console.WriteLine($"Output: {GetGraphInformation(result, sw)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed: {ex}");
                    break;
                }
            }
        }
    }
}
