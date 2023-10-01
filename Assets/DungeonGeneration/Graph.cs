using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Graph
{
    private int vertexCount;
    public int VertexCount
    {
        get { return vertexCount; }
    }

    private List<int>[] adjList;
    public List<int>[] AdjList
    {
        get { return AdjList; }
    }

    public Graph(int v1)
    {
        vertexCount = v1;
        adjList = new List<int>[vertexCount];

        for (int i = 0; i < v1; i++)
        {
            adjList[i] = new List<int>();
        }
    }

    // Adds an edge from v->w
    public void AddEdge(int v1, int w1)
    {
        adjList[v1].Add(w1);
    }

    // Removes an edge from v->w 
    public void RemoveEdge(int v1, int w1)
    {
        adjList[v1].Remove(w1);
    }

    // Breadth first traversal
    public void BFS(int s1)
        {
            bool[] visited = new bool[vertexCount];

            // Create a queue to hold the nodes that must be visited
            Queue<int> queue = new Queue<int>();
            visited[s1] = true;
            queue.Enqueue(s1);

            // Loop through all nodes in the queue
            while(queue.Count != 0)
            {
                // Dequeue a vertex and print it
                s1 = queue.Dequeue();
                Debug.Log("next->" + s1);

                // Enqueue all adjacent vertices of of s
                foreach (int next in adjList[s1])
                {
                    if(!visited[next])
                    {
                        visited[next] = true;
                        queue.Enqueue(next);
                    }
                }
            }
        }

    // Dpeth first traversal
    public void DFS(int s1)
        {
            bool[] visited = new bool[vertexCount];

            // Create a stack to hold the nodes that must be visited
            Stack<int> stack = new Stack<int>();
            visited[s1] = true;
            stack.Push(s1);

            // Loop through all nodes in the stack
            while (stack.Count != 0)
            {
                // Pop a vertex and print it
                s1 = stack.Pop();
                Debug.Log("next->" + s1);

                // Add all adjacent vertices of of s to the stack
                foreach (int next in adjList[s1])
                {
                    if (!visited[next])
                    {
                        visited[next] = true;
                        stack.Push(next);
                    }
                }
            }
        }

    public void PrintAdjacencyMatrix()
        {
            for(int i = 0; i < vertexCount; i++)
            {
                string s1 = "";
                foreach (var k in adjList[i])
                {
                    s1 = s1 + (k + ",");
                }
                s1 = s1.Substring(0, s1.Length - 1);
                s1 = i + ":[" + s1 + "]";
                Debug.Log(s1);
            }
        }
}

