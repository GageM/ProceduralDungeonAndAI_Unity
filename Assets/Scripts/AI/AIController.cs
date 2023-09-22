using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("AI/AI Controller")]
[RequireComponent(typeof(Steering))]
public class AIController : MonoBehaviour
{
    Steering steering;

    [SerializeField, Tooltip("A list of Steering Algorithms for each target in 'targets'")]
    List<State> states;

    [SerializeField, Tooltip("A list of targets for the steering algorithms")]
    List<Transform> targets;

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
        for (int i = 0; i < states.Count; i++)
        {
            steering.GetSteering(states[i], targets[i]);
        }
        steering.ClipValues();
    }
}
