using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEnlargement
{
    using QuickGraph;
    using QuickGraph.Algorithms;
    using QuickGraph.Algorithms.Services;

    public class JohnsonsAlgorithm<TVertex, TEdge> : AlgorithmBase<IVertexListGraph<TVertex, TEdge>>, IAlgorithm<IVertexListGraph<TVertex, TEdge>>, IComputation
        where TEdge : IEdge<TVertex>
    {
        private Dictionary<TVertex, bool> blocked = new Dictionary<TVertex, bool>();
        private Dictionary<TVertex, List<TVertex>> blockedVertices = new Dictionary<TVertex, List<TVertex>>();
        private List<Stack<TVertex>> cycles = new List<Stack<TVertex>>();
        private List<TVertex> vertices = new List<TVertex>();

        public JohnsonsAlgorithm(IAlgorithmComponent host, IVertexListGraph<TVertex, TEdge> visitedGraph)
            : base(host, visitedGraph)
        {
        }

        public JohnsonsAlgorithm(IVertexListGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        {
        }

        protected override void InternalCompute()
        {
            this.blocked.Clear();
            this.blockedVertices.Clear();
            Stack<TVertex> stack = new Stack<TVertex>();
            this.vertices.Clear();
            this.vertices.AddRange(this.VisitedGraph.Vertices);

            int s = 0;
            /*if (this.VisitedGraph.VertexCount > 0)
            {
                var s = LeastVertex(this.VisitedGraph);
                var done = false;
                while (!done)
                {
                    IVertexListGraph<TVertex, TEdge> subGraph = SubGraphFrom(s, this.VisitedGraph);
                    IVertexListGraph<TVertex, TEdge> leastScc = leastSCC(subGraph);
                    if (leastScc.VertexCount > 0)
                    {
                        s = LeastVertex(leastScc);

                        foreach (var vertex in leastScc.Vertices)
                        {
                            this.blocked[vertex] = false;
                            this.blockedVertices[vertex] = new List<TVertex>();
                        }

                        bool dummy = this.Circuit(leastScc, s, s, stack);
                        s++;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }*/
            
            while (s < this.vertices.Count)
            {
                IVertexListGraph<TVertex, TEdge> subGraph = SubGraphFrom(s, this.VisitedGraph);
                IVertexListGraph<TVertex, TEdge> leastScc = leastSCC(subGraph);
                if (leastScc.VertexCount > 0)
                {
                    s = LeastVertex(leastScc.Vertices);

                    foreach (var vertex in leastScc.Vertices)
                    {
                        this.blocked[vertex] = false;
                        this.blockedVertices[vertex] = new List<TVertex>();
                    }

                    bool dummy = this.Circuit(leastScc, s, s, stack);
                    s++;
                }
                else
                {
                    s = this.vertices.count;
                }
            }
        }

        private void Unblock(TVertex u)
        {
            this.blocked[u] = false;
            List<TVertex> currentVertices;
            if (this.blockedVertices.TryGetValue(u, out currentVertices))
            {
                while (currentVertices.Count > 0)
                {
                    TVertex w = currentVertices[0];
                    currentVertices.RemoveAt(0);

                    bool isBlocked;
                    if (this.blocked.TryGetValue(w, out isBlocked) && isBlocked)
                    {
                        this.Unblock(w);
                    }
                }
            }
        }

        public bool Circuit(IVertexListGraph<TVertex, TEdge> dg, TVertex v, TVertex s, Stack<TVertex> stack)
        {
            if (dg == null)
            {
                throw new ArgumentNullException(nameof(dg));
            }

            if (dg.VertexCount == 0)
            {
                return false;
            }

            bool f = false;
            stack.Push(v);
            this.blocked[v] = true;
            foreach (TVertex w in dg.OutEdges(v).Select(x => x.Target))
            {
                if (w.Equals(s))
                {
                    stack.Push(s);
                    this.cycles.Add(new Stack<TVertex>(stack));
                    stack.Pop();
                    f = true;
                }
                else
                {
                    if (!this.blocked[w])
                    {
                        if (this.Circuit(dg, w, s, stack))
                        {
                            f = true;
                        }
                    }
                }
            }

            if (f)
            {
                this.Unblock(v);
            }
            else
            {
                foreach (TVertex w in dg.OutEdges(v).Select(x => x.Target))
                {
                    List<TVertex> currentBlockedVertices;
                    if (!this.blockedVertices.TryGetValue(w, out currentBlockedVertices))
                    {
                        this.blockedVertices[w] = currentBlockedVertices = new List<TVertex>();
                    }

                    if (!currentBlockedVertices.Contains(v))
                    {
                        currentBlockedVertices.Add(v);
                    }
                }
            }

            stack.Pop();
            return f;
        }

        private TVertex LeastVertex(IEnumerable<TVertex> vertices)
        {
            int result = int.MaxValue;
            foreach (var vertex in vertices)
            {
                var idx = this.vertices.IndexOf(vertex);
                if (idx < result)
                {
                    result = idx;
                }
            }

            return this.vertices[result];
        }

        private static IVertexListGraph<TVertex, TEdge> SubGraphFrom(TVertex i, IVertexListGraph<TVertex, TEdge> inGraph)
        {
            var result = new BidirectionalGraph<TVertex, Edge<TVertex>>();
            var idFunction = inGraph.GetVertexIdentity();
            var id = idFunction(i);
            foreach (TVertex from in inGraph.Vertices)
            {
                var fromId = idFunction(from);
                if (string.Compare(fromId, id, StringComparison.Ordinal) >= 0)
                {
                    foreach (TVertex to in inGraph.OutEdges(from).Select(x => x.Target))
                    {
                        var toId = idFunction(to);
                        if (string.Compare(toId, id, StringComparison.Ordinal) >= 0)
                        {
                            if (!result.ContainsVertex(from))
                            {
                                result.AddVertex(from);
                            }

                            result.AddVertex(to);
                        }
                    }
                }
            }

            return (IVertexListGraph<TVertex, TEdge>)result;
        }

        public static DirectedGraph<Integer, WeightedEdge> leastSCC(DirectedGraph<Integer, WeightedEdge> dg) throws JohnsonIllegalStateException
        {
            Tarjan<Integer, WeightedEdge> t = new Tarjan<Integer, WeightedEdge>(dg);
    	List<List<Integer>> sccs = t.tarjan();
        Integer min = Integer.MAX_VALUE;
        List<Integer> minScc = new ArrayList<Integer>();
        for (List<Integer> scc : sccs) {
            if (scc.size() == 1) { continue; }
            for (Integer i : scc) {
                if (i<min) {
                    minScc = scc;
                    min = i;
                }
}
        }
        return addEdges(minScc, dg);
}
    }
}

namespace akCyclesInUndirectedGraphs
{
    class Program
    {
        //  Graph modelled as list of edges
        static int[,] graph =
            {
                {1, 2}, {1, 3}, {1, 4}, {2, 3},
                {3, 4}, {2, 6}, {4, 6}, {7, 8},
                {8, 9}, {9, 7}
            };

        static List<int[]> cycles = new List<int[]>();

        static void Main(string[] args)
        {
            for (int i = 0; i < graph.GetLength(0); i++)
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    findNewCycles(new int[] { graph[i, j] });
                }

            foreach (int[] cy in cycles)
            {
                string s = "" + cy[0];

                for (int i = 1; i < cy.Length; i++)
                    s += "," + cy[i];

                Console.WriteLine(s);
            }
        }

        static void findNewCycles(int[] path)
        {
            int n = path[0];
            int x;
            int[] sub = new int[path.Length + 1];

            for (int i = 0; i < graph.GetLength(0); i++)
                for (int y = 0; y <= 1; y++)
                    if (graph[i, y] == n)
                    //  edge referes to our current node
                    {
                        x = graph[i, (y + 1) % 2];
                        if (!visited(x, path))
                        //  neighbor node not on path yet
                        {
                            sub[0] = x;
                            Array.Copy(path, 0, sub, 1, path.Length);
                            //  explore extended path
                            findNewCycles(sub);
                        }
                        else if ((path.Length > 2) && (x == path[path.Length - 1]))
                        //  cycle found
                        {
                            int[] p = normalize(path);
                            int[] inv = invert(p);
                            if (isNew(p) && isNew(inv))
                                cycles.Add(p);
                        }
                    }
        }

        static bool equals(int[] a, int[] b)
        {
            bool ret = (a[0] == b[0]) && (a.Length == b.Length);

            for (int i = 1; ret && (i < a.Length); i++)
                if (a[i] != b[i])
                {
                    ret = false;
                }

            return ret;
        }

        static int[] invert(int[] path)
        {
            int[] p = new int[path.Length];

            for (int i = 0; i < path.Length; i++)
                p[i] = path[path.Length - 1 - i];

            return normalize(p);
        }

        //  rotate cycle path such that it begins with the smallest node
        static int[] normalize(int[] path)
        {
            int[] p = new int[path.Length];
            int x = smallest(path);
            int n;

            Array.Copy(path, 0, p, 0, path.Length);

            while (p[0] != x)
            {
                n = p[0];
                Array.Copy(p, 1, p, 0, p.Length - 1);
                p[p.Length - 1] = n;
            }

            return p;
        }

        static bool isNew(int[] path)
        {
            bool ret = true;

            foreach (int[] p in cycles)
                if (equals(p, path))
                {
                    ret = false;
                    break;
                }

            return ret;
        }

        static int smallest(int[] path)
        {
            int min = path[0];

            foreach (int p in path)
                if (p < min)
                    min = p;

            return min;
        }

        static bool visited(int n, int[] path)
        {
            bool ret = false;

            foreach (int p in path)
                if (p == n)
                {
                    ret = true;
                    break;
                }

            return ret;
        }
    }
}
