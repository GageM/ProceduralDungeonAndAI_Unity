using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public abstract class Action : ScriptableObject
{
    public abstract void Act(FiniteStateMachine stateMachine);
}

