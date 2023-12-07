using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(FiniteStateMachine stateMachine);
}

[CreateAssetMenu(menuName = "FSM/Decisions/Health Decision")]
public class Health_Decision : Decision
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

[CreateAssetMenu(menuName = "FSM/Decisions/Stamina Decision")]
public class Stamina_Decision : Decision
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


[CreateAssetMenu(menuName = "FSM/Decisions/Tagged Object Distance Decision")]
public class TaggedObjectDistance_Decision : Decision
{
    public float min;
    public float max;
    public string tag;
    public override bool Decide(FiniteStateMachine stateMachine)
    {
        foreach (GameObject gameObject in stateMachine.GetGameObjectsByTag(tag))
        {
            float distance = Vector3.Distance(gameObject.transform.position, stateMachine.transform.position);
            if (distance > min && distance < max)
            {
                return true;
            }
        }

        return false;
    }
}
