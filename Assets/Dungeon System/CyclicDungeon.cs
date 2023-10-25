using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CyclicDungeon : MonoBehaviour
{
    public int roomCountX = 5;
    public int roomCountY = 5;
    int RoomCount
    {
        get { return roomCountX * roomCountY; }
    }

    List<int> usableRooms = new();

    public int cycleCount = 3;

    public int maxSubCycles = 2;

    public List<GameObject> roomPlaceHolders;

    public Material cycleStartMat;
    public Material cycleGoalMat;

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

        GenerateCycle(maxSubCycles);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateCycle(int it, Cycle parent = null)
    {
        if (it > -1)
        {
            cycles = new List<Cycle>(cycleCount);

            Cycle cycle = new();

            // Create a new cycle 
            cycles.Add(cycle);

            cycle.parent = parent;

            // Decide whether the cycle is critical to complete the game
            // The main cycle is always critical
            if (it == maxSubCycles)
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

            SetUpRoomConnections(cycle);

            for (int i = 0; i < it; i++)
            {
                GenerateCycle(it - 1, cycle);
            }
        }
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
            instance.transform.position = rooms[i].position;
            rooms[i].instance = instance;
        }

        dungeonGraph = new(rooms);
    }

    void SetUpRoomConnections(Cycle cycle)
    {
        if(cycle.parent != null)
        {
            cycle.cycleStart = usableRooms[0];
            usableRooms.RemoveAt(0);

        }
        else
        {
            //cycle.cycleStart = cycle.parent.
        }

        cycle.cycleGoal = usableRooms[0];
        usableRooms.RemoveAt(0);

        dungeonGraph.nodes[cycle.cycleStart].instance.GetComponent<Renderer>().material = cycleStartMat;
        dungeonGraph.nodes[cycle.cycleGoal].instance.GetComponent<Renderer>().material = cycleGoalMat;
    }

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
    public int cycleStart;
    public int cycleGoal;

    public List<int> SubCycleInsertionPoints;

    public bool isCritical;
    public bool isSubCycle;
    public CycleType type;

    public Cycle parent;
}
