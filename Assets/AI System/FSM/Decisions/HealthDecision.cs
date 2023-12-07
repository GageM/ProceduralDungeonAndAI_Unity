using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/HealthDecision")]
public class HealthDecision : Decision
{
    public float min;
    public float max;

    public override bool Decide(FiniteStateMachine stateMachine)
    {
        var character = stateMachine.GetComponent<Character>();

        if (character.CurrentHealth > min && character.CurrentHealth < max)
        {
            return true;
        }

        return false;
    }
}
