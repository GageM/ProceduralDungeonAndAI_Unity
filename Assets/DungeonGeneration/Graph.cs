using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// A Graph Structure Using An Adjacency List
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

    public void AddEdge(int u, int v)
    {
        adjMatrix[u, v] = 1;
    }

    public void AddWeightedEdge(int u, int v, float weight)
    {
        adjMatrix[u, v] = weight;
    }

    public void RemoveEdge(int u, int v)
    {
        adjMatrix[u, v] = 0;
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

    public List<int> Dijkstra(int start, int goal)
    {
        PriorityQueue<int> frontier = new();
        frontier.Enqueue(start, 0.0f);

        // A Dictionary Containing The Node That Led To The Current Node
        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        cameFrom[start] = -1;

        // A Dictionary Containing The Cost To Rech Each Node
        Dictionary<int, float> costSoFar = new Dictionary<int, float>();
        costSoFar[start] = 0.0f;

        while(frontier.get.Count > 0)
        {
            int current = frontier.Dequeue();
            //if (current == goal) break;

            for(int i = 0; i < vertexCount; i++)
            {
                // Check For A Connection Between Nodes
                if(adjMatrix[current, i] > 0)
                {
                    // The Cost Of The Next Node Is The Cost Of The Current Node + The Weight Of The Next Node
                    float newCost = costSoFar[current] + adjMatrix[current, i];

                    Debug.Log(current + " to " + i + ": " + newCost);

                    // If There Is No Cost Associated With The Next Node
                    if (!costSoFar.ContainsKey(i))
                    {
                        costSoFar[i] = newCost;
                        float priority = newCost;
                        frontier.Enqueue(i, priority);
                        cameFrom[i] = current;
                    }
                    // If The Cost Of Moving To The Next Node is Lower Than From Another Previously Checked Node
                    else if (newCost < costSoFar[i])
                    {
                        costSoFar[i] = newCost;
                        float priority = newCost;
                        frontier.Enqueue(i, priority);
                        cameFrom[i] = current;
                    }
                }
            }
        }

        // A List that will house the path from the start node to the goal node
        List<int> result = new();


        // Trace The Path From The Goal To The Start
        int nodeIterator = goal;
        while(cameFrom[nodeIterator] != -1)
        {
            result.Add(nodeIterator);
            nodeIterator = cameFrom[nodeIterator];
        }

        //Reverse the Result To Get The Path From The Start To The Goal
        result.Reverse();

        return result;
    }

    public List<int> AStar(int start, int goal)
    {
        PriorityQueue<int> frontier = new();
        frontier.Enqueue(start, 0.0f);

        // A Dictionary Containing The Node That Led To The Current Node
        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        cameFrom[start] = -1;

        // A Dictionary Containing The Cost To Rech Each Node
        Dictionary<int, float> costSoFar = new Dictionary<int, float>();
        costSoFar[start] = 0.0f;

        while (frontier.get.Count > 0)
        {
            int current = frontier.Dequeue();
            //if (current == goal) break;

            for (int i = 0; i < vertexCount; i++)
            {
                // Check For A Connection Between Nodes
                if (adjMatrix[current, i] > 0)
                {
                    // The Cost Of The Next Node Is The Cost Of The Current Node + The Weight Of The Next Node
                    float newCost = costSoFar[current] + adjMatrix[current, i];

                    Debug.Log(current + " to " + i + ": " + newCost);

                    // If There Is No Cost Associated With The Next Node
                    if (!costSoFar.ContainsKey(i))
                    {
                        costSoFar[i] = newCost;
                        float priority = newCost;
                        frontier.Enqueue(i, priority);
                        cameFrom[i] = current;
                    }
                    // If The Cost Of Moving To The Next Node is Lower Than From Another Previously Checked Node
                    else if (newCost < costSoFar[i])
                    {
                        costSoFar[i] = newCost;
                        float priority = newCost;
                        frontier.Enqueue(i, priority);
                        cameFrom[i] = current;
                    }
                }
            }
        }

        // A List that will house the path from the start node to the goal node
        List<int> result = new();


        // Trace The Path From The Goal To The Start
        int nodeIterator = goal;
        while (cameFrom[nodeIterator] != -1)
        {
            result.Add(nodeIterator);
            nodeIterator = cameFrom[nodeIterator];
        }

        //Reverse the Result To Get The Path From The Start To The Goal
        result.Reverse();

        return result;
    }

    float manhattanHeuristic(int a, float b)
    {
        return a * b;
    }
}

