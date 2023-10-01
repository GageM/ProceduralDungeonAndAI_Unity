using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CyclicDungeon : MonoBehaviour
{
    int roomCountX = 5;
    int roomCountY = 5;

    int roomCount
    {
        get { return roomCountX * roomCountY; }
    }

    // The graph that represents the flow of the dungeon
    Graph dungeonGraph;

    // Contains the room data for each room in the graph
    List<Room> rooms;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateRooms()
    {
        dungeonGraph = new Graph(roomCount);
        for(int i = 0; i < roomCount; i++)
        {
            rooms[i] = new Room();
        }
    }
}

public struct Room
{
    CycleTheme theme;
    List<RoomType> features;
}
