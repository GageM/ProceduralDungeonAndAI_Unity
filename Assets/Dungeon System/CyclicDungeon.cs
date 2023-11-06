using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CyclicDungeon : MonoBehaviour
{
    [Header("Dungeon Parameters")]
    [SerializeField]
    public int roomCountX = 5;
    [SerializeField]
    public int roomCountY = 5;
    int RoomCount
    {
        get { return roomCountX * roomCountY; }
    }

    List<int> usableRooms = new();

    [SerializeField]
    public int cycleCount = 3;

    [SerializeField]
    int mainCycleHalfSize = 6;

    [SerializeField]
    public int maxSubCycles = 2;

    [SerializeField]
    float emptyRoomChance = 0.1f;

    [Header("Testing")]
    public List<GameObject> roomPlaceHolders;

    public Material cycleStartMat;
    public Material cycleGoalMat;
    public Material pathMat;
    public Material pathGoalMat;
    public Material emptyMat;

    [Header("Dungeon Values")]
    // Lists Containing the Starts & Goals of each cycle
    [SerializeField]
    List<Cycle> cycles;

    // The graph that represents the flow of the dungeon
    public GraphMat<Room> dungeonGraph;

    // Start is called before the first frame update
    void Start()
    {
        // Generate a list of indices that can be used to determine important rooms
        for(int i = 0; i < RoomCount; i++)
        {
            usableRooms.Add(i);
        }
        FisherYatesShuffle(usableRooms);

        InitializeGraph();

        GenerateCycle();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateCycle()
    {
        cycles = new List<Cycle>(cycleCount);
        
        Cycle cycle = new();
        
        // Create a new cycle 
        cycles.Add(cycle);
        
        // Decide whether the cycle is critical to complete the game
        // The main cycle is always critical
        if (cycles.Count == 1)
        {
            cycle.isCritical = true;
        }
        else
        {
            // Randomly set the cycle as critical
            cycle.isCritical = Random.value < 0.5;
        }
        
        // Set a random cycle type
        cycle.type = (CycleType)Random.Range(0, 12);

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------

        // Set the cycle start and goal rooms

        // Set start room
        cycle.start = usableRooms[0];
        usableRooms.RemoveAt(0);

        // Create a list of valid indices for the random walk
        List<int> validMoves = new();
        // Add each room to a list of valid moves
        for (int i = 0; i < RoomCount; i++)
        {
            validMoves.Add(i);
        }

        List<int> emptyRooms = new();
        //Remove random squares from valid moves
        for (int i = 0; i < RoomCount; i++)
        {
            if (Random.value < emptyRoomChance)
            {
                validMoves.Remove(i);
                dungeonGraph.GetNode(i).instance.GetComponent<Renderer>().material = emptyMat;
                
                // Remove edges leading to empty space
                dungeonGraph.RemoveEdgesToNode(i);
            }
        }

        List<int> path = RandomWalk(validMoves, cycle.start, mainCycleHalfSize);

        // Set goal room
        cycle.goal = path[path.Count - 1];
        usableRooms.Remove(cycle.goal);

        Debug.Log("start: " + cycle.start + " goal: " + cycle.goal);

        // Draw the path from the start to the goal
        foreach (int room in path)
        {
            dungeonGraph.GetNode(room).instance.GetComponent<Renderer>().material = pathMat;
        }

        // Remove any graph connections to the random walk path
        for(int i = 1; i < path.Count - 1; i++)
        {
            // Prevent pathfinding from overwriting path
            dungeonGraph.RemoveEdgesToNode(path[i]);
        }

        dungeonGraph.GetNode(cycle.start).instance.GetComponent<Renderer>().material = cycleStartMat;
        dungeonGraph.GetNode(cycle.goal).instance.GetComponent<Renderer>().material = cycleGoalMat;

        // Pathfind from the goal back to the start to complete the cycle
        List<int> pathBack = AStar(dungeonGraph, cycle.goal, cycle.start);
        //List<int> pathBack = dungeonGraph.Dijkstra(cycle.goal, cycle.start);

        // Catch broken cycles
        if (pathBack.Count == 0) Debug.Log("Need To Rebuild Dungeon Graph");

        // Draw the path from the goal back to the start
        foreach (int room in pathBack)
        {
            dungeonGraph.GetNode(room).instance.GetComponent<Renderer>().material = pathGoalMat;
        }

        dungeonGraph.GetNode(cycle.start).instance.GetComponent<Renderer>().material = cycleStartMat;
        dungeonGraph.GetNode(cycle.goal).instance.GetComponent<Renderer>().material = cycleGoalMat;

        // Connect the start to the goal using the path


    }

    void GenerateSubCycle()
    {

    }

    void InitializeGraph()
    {
        // Initialize The Rooms
        List<Room> rooms = new(RoomCount);
        for (int i = 0; i < roomCountY; i++)
        {
            for (int j = 0; j < roomCountX; j++)
            {
                rooms.Add(new Room());
            }
        }

        // Set the Positions of the rooms
        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].position.x = i % roomCountX;
            rooms[i].position.z = i / roomCountY;

            GameObject instance = Instantiate(roomPlaceHolders[0]);
            instance.name = "Room" + i.ToString();
            instance.transform.position = rooms[i].position;
            rooms[i].instance = instance;
        }

        dungeonGraph = new(rooms);

        ConnectAllRooms();
    }

    void ConnectAllRooms()
    {
        int up, down, left, right;
        // For Each Room
        for (int i = 0; i < RoomCount; i++)
        {
            up = i - roomCountX;
            down = i + roomCountX;
            left = i - 1;
            right = i + 1;

            // Connect Room Above
            if (up >= 0)
            {
                dungeonGraph.AddEdge(i, up);
            }
            // Connect Room Below
            if (down < RoomCount)
            {
                dungeonGraph.AddEdge(i, down);
            }
            // Connect Room Left
            if (left >= 0 && left % roomCountX != roomCountX - 1)
            {
                dungeonGraph.AddEdge(i, left);
            }
            // Connect Room Right
            if (right < RoomCount && right % roomCountX != 0)
            {
                dungeonGraph.AddEdge(i, right);
            }
        }
    }

    // Shuffling Values
    void FisherYatesShuffle(List<int> toShuffle)
    {
        System.Random rand = new();

        for(int i = toShuffle.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);

            int temp = toShuffle[i];
            toShuffle[i] = toShuffle[j];
            toShuffle[j] = temp;
        }
    }

    // Pathfinding
    public List<int> AStar(GraphMat<Room> graph, int start, int goal)
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
            int current = frontier.DequeueLowest();
            if (current == goal) break;

            foreach(int next in dungeonGraph.GetNeighbors(current))
            {
                float newCost = costSoFar[current] + dungeonGraph.GetEdgeWeight(current,next);
                if(!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + manhattanHeuristic(dungeonGraph.GetNode(goal), dungeonGraph.GetNode(next));
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

    float manhattanHeuristic(Room goal, Room next)
    {
        float D = 1f;
        float dx = Mathf.Abs(next.position.x - goal.position.x);
        float dy = Mathf.Abs(next.position.z - goal.position.z);
        return D * (dx + dy);
    }

    // Random Walk
    List<int> RandomWalk(List<int> validMoves,int start, int iterations)
    {
        int index = start;

        // Remove start from valid moves
        validMoves.Remove(index);

        // Create list to contain path and add the start to the path
        List<int> path = new();
        path.Add(index);

        for (int count = 0; count < iterations; count++)
        {
            List<int> nextValidMove = new();
            // Set possibe move
            int temp = index - roomCountX;            
            if (validMoves.Contains(temp)) nextValidMove.Add(temp);

            // Set possibe move
            temp = index + roomCountX;
            if (validMoves.Contains(temp)) nextValidMove.Add(temp);

            // Set possibe move
            temp = index - 1;
            if (validMoves.Contains(temp) && temp % roomCountX != roomCountX - 1) nextValidMove.Add(temp);

            // Set possibe move
            temp = index + 1;
            if (validMoves.Contains(temp) && temp % roomCountX != 0) nextValidMove.Add(temp);

            FisherYatesShuffle(nextValidMove);

            if (nextValidMove.Count <= 0) break;

            index = nextValidMove[0];
            validMoves.Remove(index);
            path.Add(index);

        }

        // Create edge between rooms

        // Add room to list

        // Return path from start to finish


        return path;
    }
}


[Serializable]
public class Room
{
    public Vector3 position = Vector3.zero;
    public CycleTheme theme;
    public List<RoomType> type = new();

    public GameObject instance;
}

[Serializable]
public class Cycle
{
    public int start;
    public int goal;

    public List<int> SubCycleInsertionPoints;

    public bool isCritical;
    public bool isSubCycle;
    public CycleType type;

    public Cycle parent = null;
}
