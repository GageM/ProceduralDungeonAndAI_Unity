using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "FSM/Transition")]
public class Transition : ScriptableObject
{
    State targetState;
    public State TargetState { get { return targetState; } }

    Condition condition;
    public bool isTriggered() { return condition.Test(); }
}
