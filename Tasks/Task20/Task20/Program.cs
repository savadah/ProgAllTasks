using System;
using System.Collections.Generic;

class Program
{
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
    }

    static void Task16()
    {
        Console.WriteLine("16. Поиск шарниров в графе");
    }
}