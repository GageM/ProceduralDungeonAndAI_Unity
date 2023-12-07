using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/TaggedObjectDistanceDecision")]
public class TaggedObjectDistanceDecision : Decision
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
