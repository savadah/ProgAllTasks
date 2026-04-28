using System;
using System.Collections.Generic;

class Program
{
    class Edge
    {
        public int To;
        public int Reverse;
        public int Capacity;
        public int Flow;

        public Edge(int to, int reverse, int capacity)
        {
            To = to;
            Reverse = reverse;
            Capacity = capacity;
            Flow = 0;
        }
    }

    static List<Edge>[] net;
    static int[] parV;
    static int[] parE;
    static int nNet;

    static void Main(string[] args)
    {
        Task1();

        Console.WriteLine();

        Task10();

        Console.WriteLine();

        Task16();

        Console.ReadLine();
    }

    static void Task1()
    {
        Console.WriteLine("1. Построение транзитивного замыкания с помощью DFS");

        int n = 4;

        List<int>[] graph = new List<int>[n];

        for (int i = 0; i < n; i++)
        {
            graph[i] = new List<int>();
        }

        graph[0].Add(1);
        graph[1].Add(2);
        graph[3].Add(1);

        bool[,] close = new bool[n, n];

        for (int i = 0; i < n; i++)
        {
            bool[] mark = new bool[n];

            for (int j = 0; j < graph[i].Count; j++)
            {
                int to = graph[i][j];

                if (mark[to] == false)
                {
                    DfsClose(i, to, graph, mark, close);
                }
            }
        }

        Console.WriteLine("Матрица транзитивного замыкания:");

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (close[i, j] == true)
                {
                    Console.Write("1 ");
                }
                else
                {
                    Console.Write("0 ");
                }
            }

            Console.WriteLine();
        }
    }

    static void DfsClose(int start, int v, List<int>[] graph, bool[] mark, bool[,] close)
    {
        mark[v] = true;
        close[start, v] = true;

        for (int i = 0; i < graph[v].Count; i++)
        {
            int to = graph[v][i];

            if (mark[to] == false)
            {
                DfsClose(start, to, graph, mark, close);
            }
        }
    }

    static void Task10()
    {
        Console.WriteLine("10. Построение максимального потока. Алгоритм Эдмондса-Карпа");

        nNet = 4;

        int s = 0;
        int t = 3;

        net = new List<Edge>[nNet];

        for (int i = 0; i < nNet; i++)
        {
            net[i] = new List<Edge>();
        }

        AddEdge(0, 1, 3);
        AddEdge(0, 2, 2);
        AddEdge(1, 2, 1);
        AddEdge(1, 3, 2);
        AddEdge(2, 3, 4);

        parV = new int[nNet];
        parE = new int[nNet];

        int answer = MaxFlow(s, t);

        Console.WriteLine("Максимальный поток:");
        Console.WriteLine(answer);
    }

    static void AddEdge(int from, int to, int capacity)
    {
        Edge direct = new Edge(to, net[to].Count, capacity);
        Edge back = new Edge(from, net[from].Count, 0);

        net[from].Add(direct);
        net[to].Add(back);
    }

    static bool Bfs(int s, int t)
    {
        for (int i = 0; i < nNet; i++)
        {
            parV[i] = -1;
            parE[i] = -1;
        }

        Queue<int> q = new Queue<int>();

        q.Enqueue(s);
        parV[s] = s;

        while (q.Count > 0)
        {
            int v = q.Dequeue();

            for (int i = 0; i < net[v].Count; i++)
            {
                Edge e = net[v][i];

                if (parV[e.To] == -1 && e.Capacity - e.Flow > 0)
                {
                    parV[e.To] = v;
                    parE[e.To] = i;

                    q.Enqueue(e.To);

                    if (e.To == t)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    static int MaxFlow(int s, int t)
    {
        int answer = 0;

        while (Bfs(s, t) == true)
        {
            int add = int.MaxValue;

            int cur = t;

            while (cur != s)
            {
                int prev = parV[cur];
                int edgeIndex = parE[cur];

                Edge e = net[prev][edgeIndex];

                int free = e.Capacity - e.Flow;

                if (free < add)
                {
                    add = free;
                }

                cur = prev;
            }

            cur = t;

            while (cur != s)
            {
                int prev = parV[cur];
                int edgeIndex = parE[cur];

                Edge e = net[prev][edgeIndex];

                e.Flow += add;
                net[cur][e.Reverse].Flow -= add;

                cur = prev;
            }

            answer += add;
        }

        return answer;
    }

    static void Task16()
    {
        Console.WriteLine("16. Поиск шарниров в графе");
    }
}