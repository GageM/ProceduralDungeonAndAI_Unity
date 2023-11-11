using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// A Graph Structure Using An Adjacency List
[Serializable]
public struct GraphList
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

    public GraphList(int v1)
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

// A Graph Structure Using An Adjacency Matrix
[Serializable]
public struct GraphMat<T>
{
    private int vertexCount;
    public int VertexCount { get { return vertexCount; } }

    private float[,] adjMatrix;
    public float[,] AdjMatrix { get { return adjMatrix; } }

    public List<T> nodes;

    public GraphMat(int vertexCount_)
    {
        vertexCount = vertexCount_;

        // Initialize Adjacency Matrix With No Connections
        adjMatrix = new float[vertexCount,vertexCount];

        for (int i = 0; i < vertexCount; i++) 
        {
            for (int j = 0; j < vertexCount; j++)
            {
                adjMatrix[i,j] = 0.0f;
            }
        }

        // Initialize an empty array of node values with Unknow Data
        nodes = new();
    }

    public GraphMat(List<T> nodes_)
    {
        nodes = nodes_;
        vertexCount = nodes.Count;

        // Initialize Adjacency Matrix With No Connections
        adjMatrix = new float[vertexCount, vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                adjMatrix[i, j] = 0.0f;
            }
        }
    }

    public void SetNode(T node, int index)
    {
        if((index >= 0) && (index < vertexCount - 1))
        {
            nodes[index] = node;
        }
    }

    public T GetNode(int index)
    {
        return nodes[index];
    }

    public float GetEdgeWeight(int from, int to)
    {
        return adjMatrix[from, to];
    }

    public List<int> GetNeighbors(int start)
    {
        List<int> neighbors = new();
        for(int i = 0; i < vertexCount; i++)
        {
            if (adjMatrix[start, i] > 0.0f) 
                neighbors.Add(i);
        }
        return neighbors;
    }

    public void AddEdge(int from, int to)
    {
        adjMatrix[from, to] = 1;
    }

    public void AddWeightedEdge(int from, int to, float weight)
    {
        adjMatrix[from, to] = weight;
    }

    public void RemoveEdge(int from, int to)
    {
        adjMatrix[from, to] = 0;
    }

    public void RemoveEdgesFromNode(int u)
    {
        for (int i = 0; i < vertexCount; i++)
        {
            RemoveEdge(u, i);
        }
    }

    public void RemoveEdgesToNode(int u)
    {
        for(int i = 0; i < vertexCount; i++)
        {
            RemoveEdge(i, u);
        }
    }

    public void RemoveAllEdges()
    {
        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                RemoveEdge(i, j);
            }
        }
    }

    // Prints All Nodes By BFS
    public void BFS(int start)
    {
        bool[] visited = new bool[vertexCount];
        for(int i = 0; i < visited.Length; i++) { visited[i] = false; }

        Queue<int> queue = new Queue<int>();

        queue.Enqueue(start);
        visited[start] = true;

        while(queue.Count > 0)
        {
            int currentVert = queue.Dequeue();

            string s = new("");
            s += currentVert.ToString() + ": ";

            for (int i = 0; i < vertexCount; i++)
            {
                if (adjMatrix[currentVert, i] > 0)
                {
                    s += i + ", ";
                    if (!visited[i])
                    {
                        queue.Enqueue(i);
                        visited[i] = true;
                    }
                }
            }
            Debug.Log(s);
        }
    }
}

