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
        public const int Start = 32;
        public const int End = 4096;
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
        public void TestCostOptimisedPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new CostOptimisedFirstPass(), new SecondPass());
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
        public void TestCostOptimisedSubgraphPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new Subgraph(new GraphEnlargementAlgorithmChain(new IGraphEnlargementAlgorithm[] { new CostOptimisedFirstPass(), new SecondPass() })));
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

        [TestMethod]
        public void TestCostOptimisedRevisdSubgraphPerformance()
        {
            foreach (var seed in Seeds)
            {
                var random = new Random(seed);

                ExecuteAlgorithms(random, new RevisedSubgraph(new GraphEnlargementAlgorithmChain(new IGraphEnlargementAlgorithm[] { new CostOptimisedFirstPass(), new SecondPass() })));
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

                BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> result = graph;
                try
                {
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

                if (!ValidateCycles(result))
                {
                    Assert.Fail("Output graph not valid.");
                }
            }
        }
    }
}
