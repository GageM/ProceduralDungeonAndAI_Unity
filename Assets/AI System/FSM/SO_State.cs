using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_State", menuName = "Characters/State Machine")]
public class SO_State : ScriptableObject
{
    public MoveState moveState;
    public Transition transition;
}
