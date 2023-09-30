using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("AI/AI Controller")]
[RequireComponent(typeof(Steering))]
public class AIController : MonoBehaviour
{
    Steering steering;

    [SerializeField, Tooltip("A list of Steering Algorithms for each target in 'targets'")]
    List<MoveState> moveStates;

    [SerializeField, Tooltip("A list of targets for the steering algorithms")]
    List<Transform> Movetargets;

    [SerializeField, Tooltip("The Look Steering Algorithm to use")]
    LookState lookState;

    [SerializeField, Tooltip("The focus for 'lookAtTarget' steering")]
    Transform lookTarget;

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
        for (int i = 0; i < moveStates.Count; i++)
        {
            steering.GetMovementSteering(moveStates[i], Movetargets[i]);
        }
        steering.GetLookSteering(lookState, lookTarget.position);
        steering.ClipValues();
    }
}
