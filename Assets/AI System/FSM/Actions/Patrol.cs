using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/Patrol")]
public class Patrol : Action
{
    public override void Act(FiniteStateMachine stateMachine)
    {
        var controller = stateMachine.GetComponent<AIController>();

        if (controller != null)
        {
            // check if the controller will steer to the next patrol target
            if (!controller.moveTargets.ContainsKey(controller.patrolRoute[0]))
            {
                controller.AddSteeringTarget(MoveState.ARRIVE, controller.patrolRoute[0]);
            }
            else if (controller.targetSteeringComplete[controller.patrolRoute[0]])
            {
                controller.RemoveSteeringTarget(controller.patrolRoute[0]);

                // Move the reached transform to the end of the patrol path
                controller.patrolRoute.Add(controller.patrolRoute[0]);
                controller.patrolRoute.RemoveAt(0);
            }
        }
    }
}
