using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/SteerToTaggedObject")]
public class SteerToTaggedObject : Action
{
    public string tag;
    public MoveState moveState;
    public override void Act(FiniteStateMachine stateMachine)
    {
        var controller = stateMachine.GetComponent<AIController>();
        if (controller == null) return;

        foreach (GameObject gameObject in stateMachine.GetGameObjectsByTag(tag))
        {
            if(gameObject != null)
            {
                if (!controller.moveTargets.ContainsKey(gameObject.transform))
                {
                    controller.AddSteeringTarget(moveState, gameObject.transform);
                }
            }
        }
    }
}
