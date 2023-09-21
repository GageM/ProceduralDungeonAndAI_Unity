using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/NPC Movement")]

// Require SteeringOutput component for movement
[RequireComponent (typeof(SteeringOutput))]

public class NPCMovement : MonoBehaviour
{
    SteeringOutput result;

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

    [SerializeField, Tooltip("Whether this NPC uses Dynamic or Kinematic steering algorithms")]
    public bool isKinematic;

    private void Awake()
    {
        if(!result)
        {
            result = GetComponent<SteeringOutput>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isKinematic)
        {
            transform.position += velocity * maxSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, rotation * Mathf.Rad2Deg, 0f);
        }
        else
        {
            // slow velocity if decelerating
            if(result.linearAcceleration.magnitude <= 0.001f)
            {
                velocity *= 0.95f;
            }

            transform.position += velocity * Time.deltaTime;

            transform.eulerAngles = new Vector3(0f, rotation * Mathf.Rad2Deg, 0f);

            velocity += result.linearAcceleration * Time.deltaTime;

            if(velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }
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
