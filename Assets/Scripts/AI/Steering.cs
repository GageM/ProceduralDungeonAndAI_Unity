using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteeringOutput))]
[RequireComponent(typeof(NPCMovement))]

public class Steering : MonoBehaviour
{
    SteeringOutput result;
    NPCMovement movement;

    [SerializeField, Tooltip("The target for the NPC")]
    Transform target;

    bool isKinematic;

    [SerializeField]
    float maxSpeed = 5.0f;
    [SerializeField]
    float maxAcceleration = 3.0f;

    //Used in Wander
    [SerializeField]
    float maxRotation = 1.0f;

    [SerializeField]
    float timeToTarget;

    [SerializeField]
    float radius;

    private void Awake()
    {
        if(!result)
        {
            result = GetComponent<SteeringOutput>();
        }
        if(!movement)
        {
            movement = GetComponent<NPCMovement>();
        }
        isKinematic = movement.isKinematic;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Seek();
        //Flee();
        //Arrive();
        Wander();
    }

    private float newOrientation()
    {
        if (isKinematic)
        {
            if (result.velocity.magnitude > 0f)
            {
                return Mathf.Atan2(result.velocity.x, result.velocity.z);
            }
            return transform.eulerAngles.y;
        }
        else
        {
            if(result.linearAcceleration.magnitude > 0f)
            {
                return Mathf.Atan2(result.linearAcceleration.x, result.linearAcceleration.z);
            }
            return 0;
        }
    }

    private void Seek()
    {
        if (isKinematic)
        {
            result.velocity = target.position - transform.position;
            result.velocity.y = 0;
            result.velocity = result.velocity.normalized;

            result.rotation = newOrientation();
        }
        else
        {
            result.linearAcceleration = target.position - transform.position;
            result.linearAcceleration.y = 0;

            result.linearAcceleration = result.linearAcceleration.normalized * maxAcceleration;

            result.angularAcceleration = newOrientation();
        }
    }

    private void Flee()
    {
        if (isKinematic)
        {
            result.velocity = transform.position - target.position;
            result.velocity.y = 0;
            result.velocity = result.velocity.normalized;

            result.rotation = newOrientation();
        }
        else
        {
            result.linearAcceleration = transform.position - target.position;
            result.linearAcceleration.y = 0;

            result.linearAcceleration = result.linearAcceleration.normalized * maxAcceleration;

            result.angularAcceleration = newOrientation();
        }
    }
    private void Arrive()
    {
        if (isKinematic)
        {
            result.velocity = target.position - transform.position;
            result.velocity.y = 0;

            if (result.velocity.magnitude < radius)
            {
                result.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                return;
            }

            result.velocity /= timeToTarget;

            if (result.velocity.magnitude > maxSpeed)
            {
                result.velocity = result.velocity.normalized * maxSpeed;
            }
            result.rotation = newOrientation();
        }
        else
        {

        }
    }

    private void Wander()
    {
        if(isKinematic)
        {
            result.velocity = new Vector3(Mathf.Cos(result.rotation), 0.0f, Mathf.Sin(result.rotation)) * maxSpeed;

            Random.InitState((int)System.DateTime.Now.Ticks);
            result.rotation = Random.Range(-1.0f, 1.0f) * maxRotation;

            return;
        }
    }
}
