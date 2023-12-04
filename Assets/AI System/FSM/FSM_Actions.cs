using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "FSM/Action")]
public abstract class Action : ScriptableObject
{    
    public abstract void Act();
}

public class SimpleMove : Action
{
    public MoveState moveState;
    public Transform target;

    public override void Act()
    {
        // Tell the ai controller to do this 
    }
}

public class Wait : Action
{
    public override void Act()
    {
        // Do nothing until next state is triggered
    }
}

public class FollowPath : Action
{
    public List<Transform> path;
    public bool loop;

    public override void Act()
    {
        // Tell the ai controller to follow the path
    }
}