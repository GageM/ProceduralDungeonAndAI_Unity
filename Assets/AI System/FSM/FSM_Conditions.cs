using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Condition
{
    public bool Test();
}

public class FloatCondition : I_Condition
{
    float minValue;
    float maxValue;

    public float TestValue() { return minValue; }

    public bool Test() { return (minValue <= TestValue() && TestValue() <= maxValue); }
}

public class BoolCondition : I_Condition
{
    bool value;

    public bool TestValue() { return value; }

    public bool Test() { return TestValue() == value; }
}
