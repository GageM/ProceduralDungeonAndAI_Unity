using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AI/NPC Movement")]

// Require Steering component for movement
[RequireComponent (typeof(Steering))]
[RequireComponent (typeof (Rigidbody))]

public class NPCMovement : MonoBehaviour
{
    Steering steering;
    Rigidbody rb;

    [SerializeField, Tooltip("Whether this NPC uses Dynamic or Kinematic steering algorithms")]
    public bool isKinematic;

    private void Awake()
    {
        if(!steering)
        {
            steering = GetComponent<Steering>();
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
            transform.position += steering.output.velocity * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, steering.output.rotation, 0f);
        }
        else
        {
            // slow velocity if decelerating
            if(steering.output.linearAcceleration.magnitude <= 0.001f)
            {
                rb.velocity *= 0.95f;
            }

            rb.AddForce(steering.output.linearAcceleration, ForceMode.Acceleration);

            transform.eulerAngles += new Vector3(0f, steering.output.angularAcceleration * Time.deltaTime, 0f);
        }
    }
}
