using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEnlargement
{
    using QuickGraph;
    using QuickGraph.Algorithms;

    public class J<TVertex>
    {
        private Dictionary<int, bool> blocked = new Dictionary<int, bool>();
        private Dictionary<int, List<int>> blockedNext = new Dictionary<int, List<int>>();

        private List<List<int>> cycles = new List<List<int>>();

        private Dictionary<int, TVertex> verticesMap;

        private Dictionary<TVertex, int> reverseVerticesMap;

        private IDictionary<TVertex, int> strongComponentsMap;
        private IDictionary<int, TVertex[]> reverseStrongComponentsMap;

        private void Unblock(int index)
        {
            this.blocked[index] = false;

            while (this.blockedNext[index].Count > 0)
            {
                var subIdx = this.blockedNext[index][0];
                this.blockedNext[index].RemoveAt(0);
                if (this.blocked[subIdx])
                {
                    this.Unblock(subIdx);
                }
            }
        }

        private bool Circuit(int vertex, int s, BidirectionalGraph<TVertex, Edge<TVertex>> subGraph, Stack<int> stack)
        {
            stack.Push(vertex);
            this.blocked[vertex] = true;
            var f = false;

            foreach (var edge in subGraph.OutEdges(this.verticesMap[vertex]))
            {
                int target = this.reverseVerticesMap[edge.Target];
                if (target == s)
                {
                    stack.Push(s);
                    this.cycles.Add(new List<int>(stack));
                    stack.Pop();
                    f = true;
                }
                else
                {
                    if (!this.blocked[target])
                    {
                        if (Circuit(target, s, subGraph, stack))
                        {
                            f = true;
                        }
                    }
                }
            }

            if (f)
            {
                Unblock(vertex);
            }
            else
            {
                foreach (var edge in subGraph.OutEdges(this.verticesMap[vertex]))
                {
                    int target = this.reverseVerticesMap[edge.Target];
                    if (!this.blockedNext[target].Contains(vertex))
                    {
                        this.blockedNext[target].Add(vertex);
                    }
                }
            }

            stack.Pop();
            return f;
        }

        public void Process(BidirectionalGraph<TVertex, Edge<TVertex>> graph)
        {
            graph.StronglyConnectedComponents(out this.strongComponentsMap);
            this.reverseStrongComponentsMap = this.strongComponentsMap.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Key).ToArray());

            this.verticesMap = graph.Vertices.Select((x, i) => new { Vertex = x, Index = i + 1 }).ToDictionary(x => x.Index, x => x.Vertex);
            this.reverseVerticesMap = this.verticesMap.ToDictionary(x => x.Value, x => x.Key);

            var stack = new Stack<int>();
            int s = 1;
            while (s < this.verticesMap.Count)
            {
                var subGraph = this.GetSubGraph(s, graph);

                if (subGraph.VertexCount > 0)
                {
                    s = LeastVertex(subGraph);
                    foreach (var subGraphVertex in subGraph.Vertices)
                    {
                        var i = this.reverseVerticesMap[subGraphVertex];
                        this.blocked[i] = false;
                        this.blockedNext[i] = new List<int>();
                    }

                    this.Circuit(s, s, subGraph, stack);
                    s++;
                }
                else
                {
                    s = this.verticesMap.Count;
                }
            }
        }

        private int LeastVertex(BidirectionalGraph<TVertex, Edge<TVertex>> graph)
        {
            return graph.Vertices.Select(x => this.reverseVerticesMap[x]).Min();
        }

        private BidirectionalGraph<TVertex, Edge<TVertex>> GetSubGraph(int index, BidirectionalGraph<TVertex, Edge<TVertex>> graph)
        {
            var vertices = graph.Vertices.Where(x => this.reverseVerticesMap[x] >= index);
            var edges = graph.Edges.Where(x => this.reverseVerticesMap[x.Source] >= index && this.reverseVerticesMap[x.Target] >= index).ToArray();

            var result = new BidirectionalGraph<TVertex, Edge<TVertex>>();
            result.AddVertexRange(vertices);
            result.AddEdgeRange(edges);

            // TODO: SCC

            //foreach (var vertex in vertices)
            //{
            //    var component = this.strongComponentsMap[vertex];
            //    result.AddVertexRange(this.reverseStrongComponentsMap[component]);
            //}

            return result;
        }
    }
}
