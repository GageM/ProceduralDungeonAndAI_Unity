using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/ClearSteering")]
public class ClearSteering : Action
{
    public override void Act(FiniteStateMachine stateMachine)
    {
        var controller = stateMachine.GetComponent<AIController>();

        if (controller != null)
        {
            controller.moveTargets.Clear();
            controller.targetSteeringComplete.Clear();
        }
    }
}
