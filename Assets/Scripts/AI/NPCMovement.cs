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
            transform.position += result.velocity * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, result.rotation, 0f);
        }
        else
        {
            // slow velocity if decelerating
            if(result.linearAcceleration.magnitude <= 0.001f)
            {
                rb.velocity *= 0.95f;
            }

            rb.AddForce(result.linearAcceleration, ForceMode.Acceleration);

            transform.eulerAngles += new Vector3(0f, result.angularAcceleration * Time.deltaTime, 0f);
        }
    }
}
