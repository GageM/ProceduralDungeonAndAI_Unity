using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/NPC Movement")]

// Require SteeringOutput component for movement
[RequireComponent (typeof(SteeringOutput))]
[RequireComponent (typeof (Rigidbody))]

public class NPCMovement : MonoBehaviour
{
    SteeringOutput result;
    Rigidbody rb;

    Vector3 velocity;
    public Vector3 Velocity
    {
        get { return velocity; }
    }

    float rotation;
    public float Rotation
    {
        get { return rotation; }
    }

    [SerializeField]
    float maxSpeed = 10f;
    public float MaxSpeed 
    {
        get { return maxSpeed; }
    }

    [SerializeField, Tooltip("Whether this NPC uses Dynamic or Kinematic steering algorithms")]
    public bool isKinematic;

    private void Awake()
    {
        if(!result)
        {
            result = GetComponent<SteeringOutput>();
        }
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = isKinematic;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isKinematic)
        {
            transform.position += velocity * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, rotation * Mathf.Rad2Deg, 0f);

            // set this so the velocity can be viewed by other algorithms through the rigidbody
            rb.velocity = velocity;
        }
        else
        {
            // slow velocity if decelerating
            if(result.linearAcceleration.magnitude <= 0.001f)
            {
                rb.velocity *= 0.95f;
            }

            rb.AddForce(result.linearAcceleration, ForceMode.Acceleration);

            transform.eulerAngles = new Vector3(0f, rotation * Mathf.Rad2Deg, 0f);

            velocity = result.linearAcceleration * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isKinematic)
        {
            velocity = result.velocity;
            rotation = result.rotation;
        }
        else
        {
            rotation = result.angularAcceleration;
        }

    }
}
