using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Require the character component for movement
[RequireComponent(typeof(Character))]

// Require steering component for movement
[RequireComponent (typeof(SteeringOutput))]


public class NPCMovement : MonoBehaviour
{
    Character character;
    SteeringOutput steering;

    Vector3 velocity;
    float rotation;

    [SerializeField]
    float maxKinematicSpeed = 10f;

    [SerializeField]
    float maxDynamicSpeed = 10f;

    [SerializeField]
    public bool isKinematic;

    private void Awake()
    {
        if (!character)
        {
            character = GetComponent<Character>();
            character.disableInput = true;
            character.currentSpeed = character.walkSpeed;
        }
        if(!steering)
        {
            steering = GetComponent<SteeringOutput>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isKinematic)
        {
            transform.position += velocity * maxKinematicSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, rotation * Mathf.Rad2Deg, 0f);
        }
        else
        {
            transform.position += velocity * maxKinematicSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, rotation * Mathf.Rad2Deg, 0f);

            velocity += steering.linearAcceleration * Time.deltaTime;
            rotation += steering.angularAcceleration * Time.deltaTime;

            if(velocity.magnitude > maxDynamicSpeed)
            {
                velocity = velocity.normalized * maxDynamicSpeed;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isKinematic)
        {
            velocity = steering.velocity;
            rotation = steering.rotation;
        }
        else
        {

        }

    }
}
