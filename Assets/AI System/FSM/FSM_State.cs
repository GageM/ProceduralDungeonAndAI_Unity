using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "FSM/State")]
public class State : ScriptableObject
{
    public List<Transition> transitions;

    public List<Action> actions;    

    //public List<Action> entryActions;
    //public List<Action> exitActions;
}
