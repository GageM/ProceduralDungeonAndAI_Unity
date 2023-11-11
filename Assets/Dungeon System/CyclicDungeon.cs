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

    [SerializeField]
    int mainCycleHalfSize = 6;

    [SerializeField]
    int subCycleHalfSize = 3;

    [SerializeField]
    public int maxSubCycles = 2;

    [SerializeField]
    float emptyRoomChance = 0.1f;

    [Header("Testing")]
    public List<GameObject> roomPlaceHolders;

    public Vector3 roomTileSize = Vector3.one;

    public Material defaultMat;
    public Material cycleStartMat;
    public Material cycleGoalMat;
    public Material pathMat;
    public Material pathGoalMat;

    public Material subCycleStartMat;
    public Material subCycleGoalMat;
    public Material subCyclePathMat;
    public Material subCyclePathGoalMat;

    public Material emptyMat;

    [Header("Dungeon Values")]
    // Lists Containing the Starts & Goals of each cycle
    [SerializeField]
    public List<Cycle> cycles;

    // The graph that represents the flow of the dungeon
    public GraphMat<Room> dungeonGraph;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGraph();
        GenerateCycle();
        FinalizeGraph();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Generates the main cycle for the dungeon
    bool GenerateCycle()
    {
        cycles = new();
        
        Cycle cycle = new();
        
        // Create a new cycle 
        cycles.Add(cycle);
        
        // Decide whether the cycle is critical to complete the game
        // The main cycle is always critical
        cycle.isCritical = true;
        
        // Set a random cycle type
        cycle.type = (CycleType)Random.Range(0, 12);

        // Create the cycle room list
        cycle.rooms = new();

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------

        // Set the cycle start and goal rooms

        // Set start room
        int start = Random.Range(0, RoomCount);

        //Remove random squares from valid moves to create interest
        for (int i = 0; i < RoomCount; i++)
        {
            if (Random.value < emptyRoomChance)
            {
                dungeonGraph.GetNode(i).instance.GetComponent<Renderer>().material = emptyMat;
                
                // Remove edges leading to empty space
                dungeonGraph.RemoveEdgesToNode(i);
            }
        }

        // Walk from the start to a random goal 
        List<int> path = RandomWalk(start, mainCycleHalfSize);

        //Set the cycle's rooms from the random walk
        foreach(int i in path)
        {
            cycle.rooms.Add(i);
        }

        // Set goal room index
        cycle.goalRoomIndex = cycle.rooms.Count - 1;

        Debug.Log("start: " + cycle.rooms[0] + " goal: " + cycle.rooms[cycle.goalRoomIndex]);

        // Create connections to the start room to allow pathfinding to return to it
        if (cycle.rooms.Count > 0) ConnectNeighborsToRoom(cycle.rooms[0]);

        // Pathfind from the goal back to the start to complete the cycle
        List<int> pathBack = Pathfinding.AStar(dungeonGraph, cycle.rooms[cycle.goalRoomIndex], cycle.rooms[0]);

        // Catch broken cycles
        if (pathBack.Count == 0)
        {
            Debug.Log("Need To Rebuild Dungeon Graph");
            return false;
        }

        // Draw the path from the start to the goal
        foreach (int room in path)
        {
            dungeonGraph.GetNode(room).instance.GetComponent<Renderer>().material = pathMat;
        }

        // Add the return path to the cycle's room list to complete the cycle
        foreach (int room in pathBack)
        {
            // Don't add the goal room or start room again
            if(room != cycle.rooms[cycle.goalRoomIndex] && room != cycle.rooms[0])
            {
                cycle.rooms.Add(room);
            }

            // Draw the path from the goal back to the start
            dungeonGraph.GetNode(room).instance.GetComponent<Renderer>().material = pathGoalMat;
        }

        dungeonGraph.GetNode(cycle.rooms[0]).instance.GetComponent<Renderer>().material = cycleStartMat;
        dungeonGraph.GetNode(cycle.rooms[cycle.goalRoomIndex]).instance.GetComponent<Renderer>().material = cycleGoalMat;

        // Remove any graph connections to the return path to prevent overwriting cycle
        for (int i = 1; i < pathBack.Count - 1; i++)
        {
            dungeonGraph.RemoveEdgesToNode(pathBack[i]);
        }

        // -------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------

        // Generate sub cycles
        {

            // Pick a random spot on the cycle to be the sub cycle start

            // Shuffle dungeon rooms to get sub cycle start room
            List<int> shuffledRooms = FisherYatesShuffle(cycle.rooms);

            // Remove the start and goal rooms from the shuffled list
            shuffledRooms.Remove(cycle.rooms[0]);
            shuffledRooms.Remove(cycle.rooms[cycle.goalRoomIndex]);

            // Generate the sub cycles
            while(shuffledRooms.Count > 0 && cycles.Count - 1 <= maxSubCycles)
            { 
                // Fix room connections
                ConnectAllRooms();
                foreach (Cycle c in cycles)
                {
                    foreach (int room in c.rooms)
                    {
                        dungeonGraph.RemoveEdgesToNode(room);
                    }
                }

                GenerateSubCycle(shuffledRooms[0]);
                shuffledRooms.RemoveAt(0);
            }
        }

        return true;
    }

    // Genderated the smaller cycles for the dungeons
    bool GenerateSubCycle(int subCycleStart)
    {
        Cycle cycle = new();

        // Randomly decide whether the cycle is critical to complete the game
        cycle.isCritical = Random.value > 0.5f;

        // Set a random cycle type
        cycle.type = (CycleType)Random.Range(0, 12);

        // Create the cycle room list
        cycle.rooms = new();

        //--------------------------------------------------------------------------
        //--------------------------------------------------------------------------

        // Set the cycle start and goal rooms

        // Walk from the start to a random goal 
        List<int> path = RandomWalk(subCycleStart, subCycleHalfSize);

        if(path.Count < 2)
        {
            Debug.Log("Can't generate path! Could not add cycle");
            return false;
        }

        //Set the cycle's rooms from the random walk
        foreach (int i in path)
        {
            cycle.rooms.Add(i);
        }

        // Set goal room index
        cycle.goalRoomIndex = cycle.rooms.Count - 1;

        Debug.Log("start: " + cycle.rooms[0] + " goal: " + cycle.rooms[cycle.goalRoomIndex]);

        // Create connections to the start room to allow pathfinding to return to it
        if(cycle.rooms.Count > 1) ConnectNeighborsToRoom(cycle.rooms[1]);

        // Pathfind from the goal back to the start to complete the cycle
        List<int> pathBack = Pathfinding.AStar(dungeonGraph, cycle.rooms[cycle.goalRoomIndex], cycle.rooms[1]);

        // Catch broken cycles and do not add them to the level
        if (pathBack.Count < 1)
        {
            Debug.Log("No return path! Could not add cycle");
            return false;
        }

        cycles.Add(cycle);

        // Draw the path from the start to the goal
        foreach (int room in path)
        {
            if(room != cycle.rooms[0])dungeonGraph.GetNode(room).instance.GetComponent<Renderer>().material = subCyclePathMat;
        }

        // Add the return path to the cycle's room list to complete the cycle
        foreach (int room in pathBack)
        {
            // Don't add the goal room or start room again
            if (room != cycle.rooms[cycle.goalRoomIndex] && room != cycle.rooms[0] && room != cycle.rooms[1])
            {
                cycle.rooms.Add(room);
            }

            // Draw the path from the goal back to the start
            dungeonGraph.GetNode(room).instance.GetComponent<Renderer>().material = subCyclePathGoalMat;
        }

        dungeonGraph.GetNode(cycle.rooms[1]).instance.GetComponent<Renderer>().material = subCycleStartMat;
        dungeonGraph.GetNode(cycle.rooms[cycle.goalRoomIndex]).instance.GetComponent<Renderer>().material = subCycleGoalMat;

        // Remove any graph connections to the return path to prevent overwriting cycle
        for (int i = 1; i < pathBack.Count - 1; i++)
        {
            dungeonGraph.RemoveEdgesToNode(pathBack[i]);
        }

        return true;
    }

    // Creates the final dungeon layout graph
    void FinalizeGraph()
    {
        // Clear all connections on the graph
        dungeonGraph.RemoveAllEdges();

        foreach(Cycle cycle in cycles)
        {
            for(int i = 0; i < cycle.rooms.Count; i++)
            {
                if(i < cycle.rooms.Count - 1)
                {
                    // Connect room to the next one in the cycle
                    dungeonGraph.AddEdge(cycle.rooms[i], cycle.rooms[i + 1]);

                    // Connect next room in the cycle to the current room
                    dungeonGraph.AddEdge(cycle.rooms[i + 1], cycle.rooms[i]);
                }

                // Connect the last room in a cycle to the first

                // The first room in the main cycle is index 0
                else if (cycle == cycles[0])
                {
                    dungeonGraph.AddEdge(cycle.rooms[i], cycle.rooms[0]);
                }
                // The first room in a subcycle is index 1
                else
                {
                    dungeonGraph.AddEdge(cycle.rooms[i], cycle.rooms[1]);
                }

            }
        }
    }

    // Initializes the dungeon graph
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
            rooms[i].position.x = (i % roomCountX) * roomTileSize.x;
            rooms[i].position.z = (i / roomCountY) * roomTileSize.z;

            GameObject instance = Instantiate(roomPlaceHolders[0]);
            instance.name = "Room" + i.ToString();
            instance.GetComponent<Renderer>().material = defaultMat;
            instance.transform.position = rooms[i].position;
            instance.transform.localScale = roomTileSize;
            rooms[i].instance = instance;
        }

        dungeonGraph = new(rooms);

        ConnectAllRooms();
    }

    // Creates connections between all adjacent rooms on the graph
    void ConnectAllRooms()
    {
        // For Each Room
        for (int i = 0; i < RoomCount; i++)
        {
            ConnectRoomToNeighbors(i);
        }
    }

    // Creates connections from a room to all neighboring rooms
    void ConnectRoomToNeighbors(int i)
    {
        int up = i - roomCountX;
        int down = i + roomCountX;
        int left = i - 1;
        int right = i + 1;

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

    // Creates connections from all neighboring rooms to a room
    void ConnectNeighborsToRoom(int i)
    {
        int up = i - roomCountX;
        int down = i + roomCountX;
        int left = i - 1;
        int right = i + 1;

        // Connect Room Above
        if (up >= 0)
        {
            dungeonGraph.AddEdge(up, i);
        }
        // Connect Room Below
        if (down < RoomCount)
        {
            dungeonGraph.AddEdge(down, i);
        }
        // Connect Room Left
        if (left >= 0 && left % roomCountX != roomCountX - 1)
        {
            dungeonGraph.AddEdge(left, i);
        }
        // Connect Room Right
        if (right < RoomCount && right % roomCountX != 0)
        {
            dungeonGraph.AddEdge(right, i);
        }
    }

    // Shuffling Values
    List<int> FisherYatesShuffle(List<int> toShuffle)
    {
        System.Random rand = new();
        List<int> shuffled = new();

        foreach(int i in toShuffle)
        {
            shuffled.Add(i);
        }

        for(int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);

            int temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }

        return shuffled;
    }

    // Random Walk
    List<int> RandomWalk(int start, int iterations)
    {
        int current = start;

        // Create list to contain the generated path and add the start to the path
        List<int> path = new();
        path.Add(current);

        List<int> nextValidMove = new();

        for (int count = 0; count < iterations; count++)
        {
            // Prevent random walk from returning to the current room
            dungeonGraph.RemoveEdgesToNode(current);

            // Room [x, y-1]
            int temp = current - roomCountX;            
            if (temp >= 0 && dungeonGraph.GetEdgeWeight(current, temp) > 0.0f) nextValidMove.Add(temp);

            // Room [x, y+1]
            temp = current + roomCountX;
            if (temp < RoomCount && dungeonGraph.GetEdgeWeight(current, temp) > 0.0f) nextValidMove.Add(temp);

            // Room [x-1, y]
            temp = current - 1;
            // if temp is in the array && temp does not wrap to another y value && there is a connection between rooms 
            if (temp >= 0 && temp % roomCountX != roomCountX - 1 && dungeonGraph.GetEdgeWeight(current, temp) > 0.0f) nextValidMove.Add(temp);

            // Room [x+1, y]
            temp = current + 1;
            // if temp is in the array && temp does not wrap to another y value && there is a connection between rooms 
            if (temp < RoomCount && temp % roomCountX != 0 && dungeonGraph.GetEdgeWeight(current, temp) > 0.0f) nextValidMove.Add(temp);

            nextValidMove = FisherYatesShuffle(nextValidMove);

            // Check if there are any valid moves
            if (nextValidMove.Count <= 0) break;

            current = nextValidMove[0];
            path.Add(current);

            nextValidMove.Clear();
        }
        
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
    // Stores each room in the cycle
    public List<int> rooms;
    public int goalRoomIndex;

    public List<int> SubCycleInsertionPoints;

    public bool isCritical;
    public bool isSubCycle;
    public CycleType type;

    public Cycle parent = null;
}
