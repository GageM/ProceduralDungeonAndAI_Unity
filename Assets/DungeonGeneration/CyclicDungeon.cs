using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class CyclicDungeon : MonoBehaviour
{
    int roomCountX = 5;
    int roomCountY = 5;

    int cycleCount = 3;

    // Lists Containing the Starts & Goals of each cycle
    [SerializeField]
    List<Cycle> cycleList = new List<Cycle>();

    int RoomCount
    {
        get { return roomCountX * roomCountY; }
    }

    // The graph that represents the flow of the dungeon
    public Graph dungeonGraph;

    // Contains the room data for each room in the graph
    public List<Room> rooms;


    // Start is called before the first frame update
    void Start()
    {
        GenerateRooms();
        for(int i = 0; i < cycleCount; i++)
        {
            CycleGen(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateRooms()
    {
        dungeonGraph = new Graph(RoomCount);
        rooms = new List<Room>();
        for(int i = 0; i < RoomCount; i++)
        {
            rooms.Add(new Room());
        }        
    }

    void CycleGen(int recursion)
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond * (recursion + 2));
        int rand1 = UnityEngine.Random.Range(0, RoomCount);

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond * rand1 * (recursion + 2));
        int rand2 = UnityEngine.Random.Range(0, RoomCount);

        // Ensure a cycle cannot start and end at the same room
        if(rand2 == rand1)
        {
            if (rand2 == (RoomCount - 1)) rand2 -= 1;
            else rand2 += 1;
        }

        cycleList.Add(new Cycle());

        cycleList[recursion].cycleStart = rand1;
        cycleList[recursion].cycleGoal = rand2;

        // Mark Start & Goal Rooms as such
        rooms[rand1].type.Add(RoomType.CYCLE_START);
        rooms[rand1].type.Add(RoomType.CYCLE_GOAL);
    }
}

[Serializable]
public class Room
{
    public CycleTheme theme;
    public List<RoomType> type = new();

    public Room() { }
}

[Serializable]
public class Cycle
{
    public int cycleStart;
    public int cycleGoal;

    public bool isCritical;
    CycleType type;
}
