using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringOutput : MonoBehaviour
{
    // For Kinematic Steering
    public Vector3 velocity;
    public float rotation;

    // For Dynamic Steering
    public Vector3 linearAcceleration;
    public float angularAcceleration;
}
