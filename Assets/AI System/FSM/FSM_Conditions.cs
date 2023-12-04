using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool Test();
}

public class FloatCondition : Condition
{
    float minValue;
    float maxValue;

    public float TestValue() { return minValue; }

    public override bool Test() { return (minValue <= TestValue() && TestValue() <= maxValue); }
}

public class BoolCondition : Condition
{
    bool value;

    public bool TestValue() { return value; }

    public override bool Test() { return TestValue() == value; }
}
