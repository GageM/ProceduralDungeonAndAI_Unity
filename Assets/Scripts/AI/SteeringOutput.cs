using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/Steering Output")]
public class SteeringOutput : MonoBehaviour
{
    // For Kinematic Steering
    public Vector3 velocity = Vector3.zero;
    public float rotation = 0f;

    // For Dynamic Steering
    public Vector3 linearAcceleration = Vector3.zero;
    public float angularAcceleration = 0f;
}


