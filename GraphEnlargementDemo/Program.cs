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
    using QuickGraph;
    using QuickGraph.Graphviz;

    /// <summary>
    /// Main class for the 'GraphEnlargementDemo' console application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var input = new List<MismatchedShoePerson>()
            {
                new MismatchedShoePerson() { Name = "Monde", LeftSize = 11, RightSize = 9 },
                new MismatchedShoePerson() { Name = "John", LeftSize = 12, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Hendrik", LeftSize = 10, RightSize = 8 },
                new MismatchedShoePerson() { Name = "Kefentse", LeftSize = 8, RightSize = 10 },
                new MismatchedShoePerson() { Name = "David", LeftSize = 10, RightSize = 11 },
                new MismatchedShoePerson() { Name = "Yoosuf", LeftSize = 9, RightSize = 10 },
                new MismatchedShoePerson() { Name = "Kopano", LeftSize = 7, RightSize = 6 },
                new MismatchedShoePerson() { Name = "Mark", LeftSize = 6, RightSize = 7 },
            };

            var graph = GraphBuilder.Build(input, x => x.LeftSize, x => x.RightSize);

            var graphviz = new GraphvizAlgorithm<MismatchedShoePerson, Edge<MismatchedShoePerson>>(graph);
            graphviz.FormatVertex += (sender, eventArgs) =>
                {
                    eventArgs.VertexFormatter.Label = $"{eventArgs.Vertex.Name} [{eventArgs.Vertex.LeftSize}, {eventArgs.Vertex.RightSize}]";
                };

            string output = graphviz.Generate();
            Console.WriteLine(output);
#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
#endif
        }
    }
}
