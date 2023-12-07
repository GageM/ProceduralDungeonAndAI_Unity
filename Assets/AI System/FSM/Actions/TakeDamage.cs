using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Actions/TakeDamage")]
public class TakeDamage : Action
{
    public float damageAmount;

    public override void Act(FiniteStateMachine stateMachine)
    {
        var character = stateMachine.GetComponent<Character>();

        character.TakeDamage(damageAmount);
    }
}
