using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    // The Graph's Adjacency List
    List<int> adjList;

    Graph(int i, int j)
    {
        adjList = new List<int>();
    }

}
