using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Require the character component
//[RequireComponent(typeof(Character))]
[RequireComponent (typeof(Rigidbody))]


public class NPCMovement : MonoBehaviour
{
    //Character character;
    Rigidbody rb;

    Vector3 position;  // TODO: Update with unity transform
    float orientation; // TODO: Update with unity transform

    Vector3 velocity;
    float rotation;

    float maxSpeed;

    bool isKinematic;

    SteeringOutput steering;


    private void Awake()
    {
        if(!rb)
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(isKinematic)
        {
            transform.position += steering.velocity * Time.deltaTime;
            orientation += steering.rotation * Time.deltaTime;
        }
        else
        {
            position += velocity * Time.deltaTime;
            orientation += rotation * Time.deltaTime;

            velocity += steering.linearAcceleration * Time.deltaTime;
            rotation += steering.angularAcceleration * Time.deltaTime;

            if(velocity.magnitude > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
        }
    }
}
