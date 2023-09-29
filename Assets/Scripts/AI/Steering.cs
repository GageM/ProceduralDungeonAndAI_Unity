using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State
{
    NONE = 0,
    SEEK,
    FLEE,
    ARRIVE,
    AVOID,
    PURSUE,
    EVADE
}

[AddComponentMenu("AI/Steering")]

[RequireComponent(typeof(SteeringOutput))]
[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Steering : MonoBehaviour
{
    SteeringOutput result;
    NPCMovement movement;
    Rigidbody rb;

    bool isKinematic;

    [Header("Basic Movement Stats")]
    [SerializeField]
    float maxSpeed = 5.0f;
    [SerializeField]
    float maxAcceleration = 3.0f;

    // The direction toward the target
    Vector3 direction = Vector3.zero;
    // The distance to the target
    float distance = 0.0f;

    Rigidbody targetRB;

    [HideInInspector]
    public bool arrivedAtTarget = false;

    [Header("Arrive and Avoid")]
    [SerializeField]
    float timeToTarget = 0.1f;

    [Header("Arrive Algorithm")]
    [SerializeField, Tooltip("The radius from a target to stop moving")]
    float arriveTargetRadius = 2.0f;

    [SerializeField, Tooltip("The radius from a target to start decelerating in dynamic systems")]
    float arriveSlowRadius = 5.0f;

    [Header("Avoid Algorithm")]
    [SerializeField, Tooltip("The radius from a target to stop moving")]
    float avoidTargetRadius = 10.0f;

    [SerializeField, Tooltip("The radius from a target to start decelerating in dynamic systems")]
    float avoidSlowRadius = 7.0f;

    [Header("Wander Algorithm")]
    [SerializeField]
    float maxRotation = 1.0f;

    [Header("Obstacle Avoidance")]
    [SerializeField, Tooltip("The minimum distance to an obstacle")]
    float avoidDistance = 0.5f;

    [SerializeField, Tooltip("Distance to check for obstacles")]
    float lookAhead = 2.0f;

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
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    public void GetSteering(State state_, Transform target_)
    {
        // A simple way to switch Steering algorithms in the editor
        switch (state_)
        {
            case State.SEEK:
                Seek(target_.position);
                break;
            case State.FLEE:
                Flee(target_.position);
                break;
            case State.ARRIVE:
                Arrive(target_.position);
                break;
            case State.AVOID:
                Avoid(target_.position);
                break;
            case State.PURSUE:
                Pursue(target_);
                break;
            case State.EVADE:
                Evade(target_);
                break;
            case State.NONE:
                break;
            default:
                break;
        }
    }

    private float newOrientation()
    {
        return LookWhereYouGo();
    }

    public void ClearResult()
    {
        result.velocity = Vector3.zero;
        result.rotation = 0F;

        result.linearAcceleration = Vector3.zero;
        result.angularAcceleration = 0F;
    }

    public void ClipValues()
    {
        if(isKinematic)
        {
            if (result.velocity.magnitude > maxSpeed)
            {
                result.velocity = result.velocity.normalized * maxSpeed;
            }
        }
        else
        {
            if (result.linearAcceleration.magnitude > maxAcceleration)
            {
                result.linearAcceleration = result.linearAcceleration.normalized * maxAcceleration;
            }
        }
    }

    private void Seek(Vector3 targetPos)
    {
        if (isKinematic)
        {
            direction = targetPos - transform.position;
            direction.y = 0;
            result.velocity += direction.normalized;

            result.rotation = newOrientation();
        }
        else
        {
            direction = targetPos - transform.position;
            direction.y = 0;
            result.linearAcceleration += direction.normalized * maxAcceleration;

            result.angularAcceleration = newOrientation();

        }
    }

    private void Flee(Vector3 targetPos)
    {
        if (isKinematic)
        {
            direction = transform.position - targetPos;
            direction.y = 0;
            result.velocity += direction.normalized;

            result.rotation = newOrientation();
        }
        else
        {
            direction = transform.position - targetPos;
            direction.y = 0;

            result.linearAcceleration += direction.normalized * maxAcceleration;

            result.angularAcceleration = newOrientation();
        }
    }

    private void Arrive(Vector3 targetPos)
    {
        if (isKinematic)
        {
            direction = targetPos - transform.position;
            direction.y = 0;

            if (direction.magnitude < arriveTargetRadius)
            {
                result.velocity += Vector3.zero;               
                arrivedAtTarget = true;
                return;
            }

            result.velocity = direction;

            result.rotation = newOrientation();
        }
        else
        {
            direction = targetPos - transform.position;
            direction.y = 0.0f;

            distance = direction.magnitude;

            if (distance < arriveTargetRadius)
            {
                result.linearAcceleration += Vector3.zero;
                arrivedAtTarget = true;
                return;
            }

            float targetSpeed;

            if(distance > arriveSlowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * distance / arriveSlowRadius;
            }

            Vector3 targetVelocity = direction.normalized * targetSpeed;

            result.linearAcceleration += targetVelocity - rb.velocity;

            result.angularAcceleration = newOrientation();
        }
    }

    private void Avoid(Vector3 targetPos)
    {
        if (isKinematic)
        {
            direction = transform.position - targetPos;
            direction.y = 0;

            if (direction.magnitude > avoidTargetRadius)
            {
                result.velocity += Vector3.zero;
                return;
            }

            result.velocity = direction;
            result.velocity /= timeToTarget;

            result.rotation = newOrientation();
        }
        else
        {
            direction = transform.position - targetPos;
            direction.y = 0.0f;

            distance = direction.magnitude;

            if (distance > avoidTargetRadius)
            {
                result.linearAcceleration += Vector3.zero;
                return;
            }

            float targetSpeed;

            if (distance < avoidSlowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * distance / avoidSlowRadius;
            }

            Vector3 targetVelocity = direction.normalized * targetSpeed;

            // 
            result.linearAcceleration += targetVelocity - rb.velocity;

            result.angularAcceleration = newOrientation();
        }
    }

    //TODO:: Work on Wander Algorithm
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

    private void MatchVelocity(Transform target_)
    {
        targetRB = target_.GetComponent<Rigidbody>();

        if(!targetRB)
        {
            return;
        }

        if (isKinematic)
        {
            result.velocity = targetRB.velocity;   
        }
        else
        {
            result.linearAcceleration = targetRB.velocity - rb.velocity;
            result.linearAcceleration /= timeToTarget;

            if (result.linearAcceleration.magnitude > maxAcceleration)
            {
                result.linearAcceleration = result.linearAcceleration.normalized * maxAcceleration;
            }
        }
    }

    //TODO:: Set up Align
    private void Align()
    {

    }

    //TODO:: Set up LookAtTarget
    private float LookAtTarget(Transform target_)
    {
        Vector3 direction = target_.position - transform.position;
        direction.y = 0.0f;

        if (direction.magnitude > 0f)
        {
            return Mathf.Atan2(direction.x, direction.z);
        }
        return 0;
    }

    //TODO:: Set up LookWhereYouGo
    private float LookWhereYouGo()
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

            if (rb.velocity.magnitude > 0f)
            {
                return Mathf.Atan2(rb.velocity.x, rb.velocity.z);
            }
            return 0;
        }
    }

    private void Pursue(Transform target_)
    {
        targetRB = target_.GetComponent<Rigidbody>();
        Vector3 targetPos = target_.position;

        float prediction;
        float maxPrediction = movement.MaxSpeed;

        if(targetRB)
        {
            Vector3 dir = target_.position - transform.position;
            float distance = dir.magnitude;

            float tarSpeed = targetRB.velocity.magnitude;

            if(tarSpeed <= (distance/maxPrediction))
            {
                prediction = maxPrediction;
            }
            else
            {
                prediction = distance/tarSpeed;
            }

            targetPos += targetRB.velocity * prediction;
        }

        Arrive(targetPos);
    }

    private void Evade(Transform target_)
    {
        targetRB = target_.GetComponent<Rigidbody>();
        Vector3 targetPos = target_.position;

        float prediction;
        float maxPrediction = movement.MaxSpeed;

        if (targetRB)
        {
            Vector3 dir = transform.position - target_.position;
            float distance = dir.magnitude;

            float tarSpeed = targetRB.velocity.magnitude;

            if (tarSpeed <= (distance / maxPrediction))
            {
                prediction = maxPrediction;
            }
            else
            {
                prediction = distance / tarSpeed;
            }

            targetPos += targetRB.velocity * prediction;
        }

        Avoid(targetPos);
    }
}
