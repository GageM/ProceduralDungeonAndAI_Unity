using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding
{
    // Pathfinding
    public static List<int> AStar(GraphMat<Room> graph, int start, int goal)
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
            int current = frontier.DequeueLowest();
            if (current == goal) break;

            foreach (int next in graph.GetNeighbors(current))
            {
                float newCost = costSoFar[current] + graph.GetEdgeWeight(current, next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + manhattanHeuristic(graph.GetNode(goal), graph.GetNode(next));
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // A List that will house the path from the start node to the goal node
        List<int> result = new();

        // Trace The Path From The Goal To The Start
        int nodeIterator = goal;

        // Catches no possible path issues
        if (nodeIterator != -1 && !cameFrom.ContainsKey(nodeIterator))
        {
            Debug.Log("No Path Available");
            return new();
        }

        if (cameFrom.ContainsKey(nodeIterator))
        {
            while (cameFrom[nodeIterator] != -1)
            {
                result.Add(nodeIterator);
                nodeIterator = cameFrom[nodeIterator];
            }
            //Reverse the Result To Get The Path From The Start To The Goal
            result.Reverse();
        }
        return result;
    }

    public static List<int> Dijkstra(GraphMat<Room> graph, int start, int goal)
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
            int current = frontier.DequeueLowest();
            if (current == goal) break;

            foreach (int next in graph.GetNeighbors(current))
            {
                float newCost = costSoFar[current] + graph.GetEdgeWeight(current, next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost;
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        // A List that will house the path from the start node to the goal node
        List<int> result = new();

        // Trace The Path From The Goal To The Start
        int nodeIterator = goal;

        // Catches no possible path issues
        if (nodeIterator != -1 && !cameFrom.ContainsKey(nodeIterator))
        {
            Debug.Log("No Path Available");
            return new();
        }

        if (cameFrom.ContainsKey(nodeIterator))
        {
            while (cameFrom[nodeIterator] != -1)
            {
                result.Add(nodeIterator);
                nodeIterator = cameFrom[nodeIterator];
            }
            //Reverse the Result To Get The Path From The Start To The Goal
            result.Reverse();
        }
        return result;
    }

    static float manhattanHeuristic(Room goal, Room next)
    {
        float D = 1f;
        float dx = Mathf.Abs(next.position.x - goal.position.x);
        float dy = Mathf.Abs(next.position.z - goal.position.z);
        return D * (dx + dy);
    }
}
