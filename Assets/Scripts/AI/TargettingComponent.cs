using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/Targetting Component")]

[RequireComponent(typeof(Steering))]
public class TargettingComponent : MonoBehaviour
{
    // A ref to the steering component to control its target
    Steering steering;

    [SerializeField, Tooltip("A list of targets for the steering algorithms")]
    List<Transform> targets;

    int currentTargetIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if(!steering)
        {
            steering = GetComponent<Steering>();
            steering.target = targets[currentTargetIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        AssignTarget();
    }

    void AssignTarget()
    {
        if(steering.arrivedAtTarget)
        {
            SelectNextTarget();
            steering.arrivedAtTarget = false;
            steering.target = targets[currentTargetIndex];
        }
    }

    void SelectNextTarget()
    {
        currentTargetIndex++;
        if(currentTargetIndex == targets.Count)
        {
            currentTargetIndex = 0;
        }


    }
}
