using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    [SerializeField]
    public State initialState;
    
    State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = initialState;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any transitions have been triggered
        foreach(var transition in currentState.transitions)
        {
            if(transition.isTriggered())
            {
                currentState = transition.TargetState;
                break;
            }
        }

        // Perform actions for current state
        foreach(var action in currentState.actions)
        {
            action.Act();
        }
    }
}






