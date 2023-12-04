using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class State
{
    public List<Transition> transitions;

    public List<I_Action> actions;

    //public List<I_Action> entryActions;

    //public List<I_Action> exitActions;
}
