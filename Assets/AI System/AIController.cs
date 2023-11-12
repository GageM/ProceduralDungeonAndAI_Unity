using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("AI/AI Controller")]
[RequireComponent(typeof(Steering))]
public class AIController : MonoBehaviour
{
    // Steering
    Steering steering;

    [SerializeField, Tooltip("A list of Steering Algorithms for each target in 'targets'")]
    List<MoveState> moveStates;

    [SerializeField, Tooltip("A list of targets for the steering algorithms")]
    List<Transform> Movetargets;

    [SerializeField, Tooltip("The Look Steering Algorithm to use")]
    LookState lookState;

    [SerializeField, Tooltip("The focus for 'lookAtTarget' steering")]
    Transform lookTarget;

    // Pathfinding
    [Header("Pathfinding")]
    [SerializeField]
    bool usePathfinding = false;

    // A flag for if we have reached our goal
    bool reachedGoal = true;

    public bool foundPath = true;

    [SerializeField]
    public CyclicDungeon dungeon;

    public List<int> path;

    int startindex;
    public GameObject start;

    int goalindex;
    public GameObject goal;

    public int currentTarget;

    public Transform currentTargetTransform;

    // Start is called before the first frame update
    void Awake()
    {
        if(!steering)
        {
            steering = GetComponent<Steering>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        steering.ClearResult();
        // Run the steering algorithms
        
        // Run default steering algorithms
        if (!usePathfinding)
        {
            for (int i = 0; i < moveStates.Count; i++)
            {
                steering.GetMovementSteering(moveStates[i], Movetargets[i]);
            }
        }
        else
        {
            // Check if we have a path to follow
            if (!foundPath)
            {
                path.Clear();

                for(int i = 0; i < dungeon.RoomCount; i++)
                {
                    if(start.transform.position == dungeon.dungeonGraph.GetNode(i).position)
                    {
                        startindex = i;
                    }
                    if (goal.transform.position == dungeon.dungeonGraph.GetNode(i).position)
                    {
                        goalindex = i;
                    }
                }

                path = Pathfinding.AStar(dungeon.dungeonGraph, startindex, goalindex);

                if (path.Count > 0)
                {
                    // Get the first node along the path
                    currentTarget = path[0];
                    currentTargetTransform = dungeon.dungeonGraph.nodes[currentTarget].instance.transform;
                }

                foundPath = true;  
                reachedGoal = false;
            }

            // Arrive at next node on the path
            if (!reachedGoal && foundPath)
            {
                if(path.Count > 0)
                {
                    // Check if we have arrived at the next node on the path
                    if (steering.GetMovementSteering(MoveState.ARRIVE, currentTargetTransform))
                    {
                        // Check if there is another node on the path to move to
                        if (path.Count > 1)
                        {
                            path.RemoveAt(0);

                            // Get the next node along the path
                            currentTarget = path[0];
                            currentTargetTransform = dungeon.dungeonGraph.nodes[currentTarget].instance.transform;
                        }

                        else
                        {
                            reachedGoal = true;
                        }
                    }

                }
                else
                {
                    reachedGoal = true;
                }
            }    
        }


        steering.GetLookSteering(lookState, lookTarget.position);
        steering.ObstacleAvoidance();
        steering.CalculateOutput();
    }
}
