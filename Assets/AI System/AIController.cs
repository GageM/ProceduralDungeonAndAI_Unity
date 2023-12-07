using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("AI/AI Controller")]
[RequireComponent(typeof(Steering))]
public class AIController : MonoBehaviour
{
    // Steering
    Steering steering;

    // Contains a set of targets and how to steer relative to them
    public Dictionary<Transform, MoveState> moveTargets = new();

    // Contains flags on whether a steering action is complete
    public Dictionary<Transform, bool> targetSteeringComplete = new();

    [Header("Steering")]
    [SerializeField, Tooltip("The Look Steering Algorithm to use")]
    LookState lookState;

    [SerializeField, Tooltip("The focus for 'lookAtTarget' steering")]
    Transform lookTarget;

    // Pathfinding
    [Header("Pathfinding")]
    [SerializeField]
    bool usePathfinding = false;

    // A flag for if we have reached our goal
    [HideInInspector]
    public bool reachedGoal = true;

    public bool foundPath = true;

    [SerializeField]
    public CyclicDungeon dungeon;

    public List<int> pathfindingPath;

    int startindex;
    public GameObject start;

    int goalindex;
    public GameObject goal;

    public int currentTarget;

    public Transform currentTargetTransform;

    [Header("Decision Making")]
    [SerializeField]
    public List<Transform> patrolRoute;

    // Start is called before the first frame update
    void Awake()
    {
        if(!steering)
        {
            steering = GetComponent<Steering>();
        }
    }

    void FixedUpdate()
    {
        // Initialize steering for this update cycle
        steering.ClearResult();

        foreach(var kvp in moveTargets)
        {
            targetSteeringComplete[kvp.Key] = steering.GetMovementSteering(kvp.Value, kvp.Key);
        }        

        // Control look steering
        steering.GetLookSteering(lookState, lookTarget);

        // Obstacle avoidance
        steering.ObstacleAvoidance();

        // Calculate the steering ouput used by NPC Movement
        steering.CalculateOutput();
    }

    // Add & Remove Steering Target are used by FSM Actions to control the NPC

    public void AddSteeringTarget(MoveState state, Transform target)
    {
        if (!moveTargets.ContainsKey(target))
        {
            moveTargets.Add(target, state);
            targetSteeringComplete.Add(target, false);
        }
    }

    public void RemoveSteeringTarget(Transform target)
    {
        if (moveTargets.ContainsKey(target))
        {
            moveTargets.Remove(target);
            targetSteeringComplete.Remove(target);  
        }
    }

    void RunPathfinding()
    {

        // Check if we have a path to follow
        if (!foundPath)
        {
            pathfindingPath.Clear();

            for (int i = 0; i < dungeon.RoomCount; i++)
            {
                if (start.transform.position == dungeon.dungeonGraph.GetNode(i).position)
                {
                    startindex = i;
                }
                if (goal.transform.position == dungeon.dungeonGraph.GetNode(i).position)
                {
                    goalindex = i;
                }
            }

            pathfindingPath = Pathfinding.AStar(dungeon.dungeonGraph, startindex, goalindex);

            if (pathfindingPath.Count > 0)
            {
                // Get the first node along the path
                currentTarget = pathfindingPath[0];
                currentTargetTransform = dungeon.dungeonGraph.nodes[currentTarget].instance.transform;
            }

            foundPath = true;
            reachedGoal = false;
        }

        // Arrive at next node on the path
        if (!reachedGoal && foundPath)
        {
            if (pathfindingPath.Count > 0)
            {
                // Check if we have arrived at the next node on the path
                if (steering.GetMovementSteering(MoveState.ARRIVE, currentTargetTransform))
                {
                    // Check if there is another node on the path to move to
                    if (pathfindingPath.Count > 1)
                    {
                        pathfindingPath.RemoveAt(0);

                        // Get the next node along the path
                        currentTarget = pathfindingPath[0];
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
}
