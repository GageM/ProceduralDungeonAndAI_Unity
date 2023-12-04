using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class Transition
{
    //List<I_Action> actions;
    //public List<I_Action> Actions { get { return actions; } }

    State targetState;
    public State TargetState { get { return targetState; } }

    I_Condition condition;

    public bool isTriggered() { return condition.Test(); }
}
