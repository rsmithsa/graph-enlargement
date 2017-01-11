//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Richard Smith">
//     Copyright (c) Richard Smith. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace GraphEnlargementDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GraphEnlargement;
    using GraphEnlargement.Sanders;
    using GraphEnlargement.Smith;
    using GraphEnlargement.VanDerLinde;
    using QuickGraph;
    using QuickGraph.Graphviz;

    /// <summary>
    /// Main class for the 'GraphEnlargementDemo' console application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = GenerateInput(50, 42, true);
            /*var input = new List<MismatchedShoePerson>()
            {
                new MismatchedShoePerson() { Name = "Monde", LeftSize = 11, RightSize = 9 },
                new MismatchedShoePerson() { Name = "John", LeftSize = 12, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Hendrik", LeftSize = 10, RightSize = 8 },
                new MismatchedShoePerson() { Name = "Kefentse", LeftSize = 8, RightSize = 10 },
                new MismatchedShoePerson() { Name = "David", LeftSize = 10, RightSize = 11 },
                new MismatchedShoePerson() { Name = "Yoosuf", LeftSize = 9, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Kopano", LeftSize = 7, RightSize = 6 },
                new MismatchedShoePerson() { Name = "Mark", LeftSize = 6, RightSize = 7 },
            };*/

            /*var input = new List<MismatchedShoePerson>()
            {
                new MismatchedShoePerson() { Name = "Adam", LeftSize = 7, RightSize = 5 },
                new MismatchedShoePerson() { Name = "Bob", LeftSize = 8, RightSize = 7 },
                new MismatchedShoePerson() { Name = "Carol", LeftSize = 8, RightSize = 9 },
                new MismatchedShoePerson() { Name = "David", LeftSize = 9, RightSize = 8 },
                new MismatchedShoePerson() { Name = "Eddie", LeftSize = 9, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Frank", LeftSize = 10, RightSize = 8 },
                new MismatchedShoePerson() { Name = "George", LeftSize = 12, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Harry", LeftSize = 13, RightSize = 11 },
                new MismatchedShoePerson() { Name = "Ike", LeftSize = 5, RightSize = 12 },
            };*/

            /*var input = new List<MismatchedShoePerson>()
            {
                new MismatchedShoePerson() { Name = "Adam", LeftSize = 7, RightSize = 8 },
                new MismatchedShoePerson() { Name = "Bob", LeftSize = 8, RightSize = 7 },
                new MismatchedShoePerson() { Name = "Carol", LeftSize = 8, RightSize = 9 },
                new MismatchedShoePerson() { Name = "David", LeftSize = 8, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Eddie", LeftSize = 8, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Frank", LeftSize = 9, RightSize = 7 },
                new MismatchedShoePerson() { Name = "George", LeftSize = 10, RightSize = 8 },
                new MismatchedShoePerson() { Name = "Harry", LeftSize = 10, RightSize = 12 },
                new MismatchedShoePerson() { Name = "Ike", LeftSize = 11, RightSize = 12 },
                new MismatchedShoePerson() { Name = "Jim", LeftSize = 11, RightSize = 13 },
                new MismatchedShoePerson() { Name = "Kenny", LeftSize = 12, RightSize = 11 },
                new MismatchedShoePerson() { Name = "Larry", LeftSize = 12, RightSize = 14 },
            };*/

            var graph = GraphBuilder.Build(input, x => x.LeftSize, x => x.RightSize);
            var inputDot = GenerateGraphvizDot(graph);
            Console.WriteLine($"Input: {GetGraphInformation(graph)}");

            var permutation = new Permutation(false)
                .Apply(graph, (name, inVertex, outVertex) => new MismatchedShoePerson() { Name = name, LeftSize = outVertex.RightSize, RightSize = inVertex.LeftSize });
            var permutationDot = GenerateGraphvizDot(permutation);
            Console.WriteLine($"Permutation: {GetGraphInformation(permutation)}");

            var permutationPrune = new Permutation(true)
                .Apply(graph, (name, inVertex, outVertex) => new MismatchedShoePerson() { Name = name, LeftSize = outVertex.RightSize, RightSize = inVertex.LeftSize });
            var permutationPruneDot = GenerateGraphvizDot(permutationPrune);
            Console.WriteLine($"Permutation Pruned: {GetGraphInformation(permutationPrune)}");

            var revisedSubGraphPermutation = new RevisedSubGraph(new Permutation())
                .Apply(graph, (name, inVertex, outVertex) => new MismatchedShoePerson() { Name = name, LeftSize = outVertex.RightSize, RightSize = inVertex.LeftSize });
            var revisedSubGraphPermutationDot = GenerateGraphvizDot(revisedSubGraphPermutation);
            Console.WriteLine($"Revised Sub Graph - Permutation: {GetGraphInformation(revisedSubGraphPermutation)}");

            /*var secondPass = new SecondPass().Apply(firstPass, (name, inVertex, outVertex) => new MismatchedShoePerson() { Name = name, LeftSize = outVertex.RightSize, RightSize = inVertex.LeftSize });
            Console.WriteLine(GenerateGraphvizDot(secondPass));
            Console.WriteLine();*/

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
#endif
        }

        private static string GenerateGraphvizDot(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
        {
            var graphviz = new GraphvizAlgorithm<MismatchedShoePerson, Edge<MismatchedShoePerson>>(graph);
            graphviz.FormatVertex += (sender, eventArgs) => { eventArgs.VertexFormatter.Label = eventArgs.Vertex.ToString(); };

            return graphviz.Generate();
        }

        private static string GetGraphInformation(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
        {
            return $"{graph.GetDescription()}{Environment.NewLine}Edges Valid: {(ValidateEdges(graph) ? "Yes" : "No")}{Environment.NewLine}Cycles Valid: {(ValidateCycles(graph) ? "Yes" : "No")}{Environment.NewLine}";
        }

        private static bool ValidateCycles(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
        {
            return graph.GetVerticesNotInCycles().Count == 0;
        }

        private static bool ValidateEdges(BidirectionalGraph<MismatchedShoePerson, Edge<MismatchedShoePerson>> graph)
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

        private static List<MismatchedShoePerson> GenerateInput(int count, int seed, bool excludeSelfCycles)
        {
            var result = new List<MismatchedShoePerson>();
            var random = new Random(seed);

            for (int i = 0; i < count; i++)
            {
                var item = new MismatchedShoePerson() { Name = $"Node {i + 1}", LeftSize = random.Next(1, 30), RightSize = random.Next(1, 30) };
                if (excludeSelfCycles && item.LeftSize == item.RightSize)
                {
                    i--;
                    continue;
                }

                result.Add(item);
            }

            return result;
        }
    }
}
