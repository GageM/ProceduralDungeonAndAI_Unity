using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoveState
{
    NONE = 0,
    SEEK,
    FLEE,
    ARRIVE,
    AVOID,
    PURSUE,
    EVADE
}

public enum LookState
{
    NONE = 0,
    LOOK_AT_TARGET,
    LOOK_WHERE_MOVING,

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
    [SerializeField, Tooltip("The maximum speed the NPC can move")]
    float maxSpeed = 5.0f;
    [SerializeField, Tooltip("The maximum amount of angular acceleration that can be applied at once")]
    float maxAcceleration = 3.0f;
    [SerializeField, Tooltip("The maximum speed the NPC can turn (in degrees)")]
    float maxRotation = 5.0f;
    [SerializeField, Tooltip("The maximum amount of angular acceleration that can be applied at once (in degrees)")]
    float maxAngularAcceleration = 3.0f;

    // The direction toward the target
    Vector3 direction = Vector3.zero;
    // The distance to the target
    float distance = 0.0f;

    Rigidbody targetRB;

    [HideInInspector]
    public bool arrivedAtTarget = false;

    [Header("Shared")]
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

    [Header("Align Algorithm")]
    [SerializeField, Tooltip("The difference in rotation from target to stop turning")]
    float alignTargetRadius = 10.0f;

    [SerializeField, Tooltip("The difference in rotation from a target to slow turning")]
    float alignSlowRadius = 5.0f;

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

    public void GetMovementSteering(MoveState moveState_,Transform target_)
    {
        // Controls The Movement Steering Of The NPC
        switch (moveState_)
        {
            case MoveState.SEEK:
                Seek(target_.position);
                break;
            case MoveState.FLEE:
                Flee(target_.position);
                break;
            case MoveState.ARRIVE:
                Arrive(target_.position);
                break;
            case MoveState.AVOID:
                Avoid(target_.position);
                break;
            case MoveState.PURSUE:
                Pursue(target_);
                break;
            case MoveState.EVADE:
                Evade(target_);
                break;
            case MoveState.NONE:
                break;
            default:
                break;
        }
    }

    public void GetLookSteering(LookState lookState_, Vector3 target_)
    {
        switch (lookState_)
        {
            case LookState.LOOK_WHERE_MOVING:
                LookWhereYouGo();
                break;

            case LookState.LOOK_AT_TARGET:
                LookAtTarget(target_);
                break;

            case LookState.NONE:
                break;

            default:
                break;
        }
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

            float angularAcceleration = Mathf.Abs(result.angularAcceleration);

            if (angularAcceleration > maxAngularAcceleration)
            {
                result.angularAcceleration /= angularAcceleration;
                result.angularAcceleration *= maxAngularAcceleration;
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
        }
        else
        {
            direction = targetPos - transform.position;
            direction.y = 0;
            result.linearAcceleration += direction.normalized * maxAcceleration;

        }
    }

    private void Flee(Vector3 targetPos)
    {
        if (isKinematic)
        {
            direction = transform.position - targetPos;
            direction.y = 0;
            result.velocity += direction.normalized;
        }
        else
        {
            direction = transform.position - targetPos;
            direction.y = 0;

            result.linearAcceleration += direction.normalized * maxAcceleration;
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
    private void Align(float targetOrientation)
    {
        if(isKinematic)
        {
            // Set the rotation to be the target rotation
            result.rotation = targetOrientation;
        }
        else
        {
            float rotation = targetOrientation - transform.rotation.eulerAngles.y;
            rotation = MapAngleToRange(rotation);

            float rotationSize = Mathf.Abs(rotation);

            if (rotationSize < alignTargetRadius)
            {
                return;
            }

            float targetRotation;

            if (rotationSize > alignSlowRadius) targetRotation = maxRotation;
            else targetRotation = maxRotation * rotationSize / alignSlowRadius;

            targetRotation *= rotation / rotationSize;

            result.angularAcceleration = targetRotation;
            result.angularAcceleration /= timeToTarget;            
        }
    }

    float MapAngleToRange(float angle)
    {
        float r = angle % 360;

        if (r < -180) r += 360;
        if (r > 180) r -= 360;
        return r;
    }

    //TODO:: Set up LookAtTarget
    private void LookAtTarget(Vector3 target_)
    {
        Vector3 direction = target_ - transform.position;
        direction.y = 0.0f;

        if (direction.magnitude > 0f)
        {
            Align(Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
        }
        return;
    }

    //TODO:: Set up LookWhereYouGo
    private void LookWhereYouGo()
    {
        if(isKinematic && result.velocity.magnitude > 0.2f) Align(Mathf.Atan2(result.velocity.x, result.velocity.z) * Mathf.Rad2Deg);
        else if (rb.velocity.magnitude > 0.2f) Align(Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg);

        return;
    }

    private void Pursue(Transform target_)
    {
        targetRB = target_.GetComponent<Rigidbody>();
        Vector3 targetPos = target_.position;

        float prediction;
        float maxPrediction = maxSpeed;

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
        float maxPrediction = maxSpeed;

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
