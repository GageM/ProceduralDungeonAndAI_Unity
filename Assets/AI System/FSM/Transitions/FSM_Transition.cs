using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "FSM/Transition")]
public class Transition : ScriptableObject
{
    public State targetState;
    public Decision decision;

    public bool Test(FiniteStateMachine stateMachine) 
    { 
        return decision.Decide(stateMachine);
    }
}
