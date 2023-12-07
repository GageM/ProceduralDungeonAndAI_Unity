using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/StaminaDecision")]
public class StaminaDecision : Decision
{
    public float min;
    public float max;

    public override bool Decide(FiniteStateMachine stateMachine)
    {
        var character = stateMachine.GetComponent<Character>();

        if (character.CurrentStamina > min && character.CurrentStamina < max)
        {
            return true;
        }

        return false;
    }
}
