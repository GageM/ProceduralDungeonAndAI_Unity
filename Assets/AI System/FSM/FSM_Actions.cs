using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public interface I_Action
{
    public void Act();
}

public class SimpleMove : I_Action
{
    public MoveState moveState;
    public Transform target;

    public void Act()
    {
        // Tell the ai controller to do this 
    }
}

public class Wait : I_Action
{

    public void Act()
    {
        // Do nothing until next state is triggered
    }
}

public class FollowPath : I_Action
{
    public List<Transform> path;
    public bool loop;

    public void Act()
    {
        // Tell the ai controller to follow the path
    }
}