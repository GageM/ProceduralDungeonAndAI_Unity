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

[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Steering : MonoBehaviour
{
    public List<SteeringOutput> results;

    public SteeringOutput output;

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
    float avoidDistance = 2.0f;

    [SerializeField, Tooltip("Distance to check for obstacles")]
    float lookAhead = 5.0f;

    [SerializeField, Tooltip("The 'Whisker' angle")]
    float whiskerAngle = 90f;

    private void Awake()
    {
        results = new List<SteeringOutput>();
        output = new SteeringOutput();
        if (!movement)
        {
            movement = GetComponent<NPCMovement>();
        }
        isKinematic = movement.isKinematic;
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    public void GetMovementSteering(MoveState moveState_, Transform target_)
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
        results.Clear();
        output.velocity = Vector3.zero;
        output.linearAcceleration = Vector3.zero;

        output.rotation = 0.0f;
        output.angularAcceleration = 0.0f;
    }

    public void ClipValues()
    {
        if (isKinematic)
        {
            if (output.velocity.magnitude > maxSpeed)
            {
                output.velocity = output.velocity.normalized * maxSpeed;
            }
        }
        else
        {
            if (output.linearAcceleration.magnitude > maxAcceleration)
            {
                output.linearAcceleration = output.linearAcceleration.normalized * maxAcceleration;
                float angularAcceleration = Mathf.Abs(output.angularAcceleration);

                if (angularAcceleration > maxAngularAcceleration)
                {
                    output.angularAcceleration /= angularAcceleration;
                    output.angularAcceleration *= maxAngularAcceleration;
                }
            }
        }
    }
    public void CalculateOutput()
    {
        float totalWeight = 0.0f;
        foreach (var result in results)
        {
            totalWeight += result.weight;
        }

        foreach (var result in results)
        {
            result.weight /= totalWeight;

            if (result.weight > 0.1f)
            {
                output.velocity += result.velocity * result.weight;
                output.rotation += result.rotation * result.weight;

                output.linearAcceleration += result.linearAcceleration * result.weight;
                output.angularAcceleration += result.angularAcceleration * result.weight;
            }
        }

        output.weight = 1.0f;
        ClipValues();
    }

    private void Seek(Vector3 targetPos, float weight = 1.0f)
    {
        SteeringOutput result = new SteeringOutput();
        result.weight = weight;
        if (isKinematic)
        {
            direction = targetPos - transform.position;
            direction.y = 0;
            result.velocity = direction.normalized;
        }
        else
        {
            direction = targetPos - transform.position;
            direction.y = 0;
            result.linearAcceleration = direction.normalized * maxAcceleration;
            results.Add(result);
        }
    }

    private void Flee(Vector3 targetPos, float weight = 1.0f)
    {
        SteeringOutput result = new SteeringOutput();
        result.weight = weight;
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

            result.linearAcceleration = direction.normalized * maxAcceleration;
        }
        results.Add(result);
    }

    private void Arrive(Vector3 targetPos, float weight = 1.0f)
    {
        SteeringOutput result = new SteeringOutput();
        result.weight = weight;
        if (isKinematic)
        {
            direction = targetPos - transform.position;
            direction.y = 0;

            if (direction.magnitude < arriveTargetRadius)
            {
                result.velocity = Vector3.zero;
                arrivedAtTarget = true;
                results.Add(result);
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
                result.linearAcceleration = Vector3.zero;
                arrivedAtTarget = true;
                results.Add(result);
                return;
            }

            float targetSpeed;

            if (distance > arriveSlowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = maxSpeed * distance / arriveSlowRadius;
            }

            Vector3 targetVelocity = direction.normalized * targetSpeed;

            result.linearAcceleration = targetVelocity - rb.velocity;
        }
        results.Add(result);
    }

    private void Avoid(Vector3 targetPos, float weight = 1.0f)
    {
        SteeringOutput result = new SteeringOutput();
        result.weight = weight;
        if (isKinematic)
        {
            direction = transform.position - targetPos;
            direction.y = 0;

            if (direction.magnitude > avoidTargetRadius)
            {
                result.velocity = Vector3.zero;
                results.Add(result);
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
                result.linearAcceleration = Vector3.zero;
                results.Add(result);
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
            result.linearAcceleration = targetVelocity - rb.velocity;
        }
        results.Add(result);
    }

    //TODO:: Work on Wander Algorithm
    private void Wander()
    {
        SteeringOutput result = new SteeringOutput();
        if (isKinematic)
        {
            result.velocity = new Vector3(Mathf.Cos(result.rotation), 0.0f, Mathf.Sin(result.rotation)) * maxSpeed;

            Random.InitState((int)System.DateTime.Now.Ticks);
            result.rotation = Random.Range(-1.0f, 1.0f) * maxRotation;

            return;
        }
        results.Add(result);
    }

    private void MatchVelocity(Transform target_)
    {
        SteeringOutput result = new SteeringOutput();
        targetRB = target_.GetComponent<Rigidbody>();

        if (!targetRB)
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
        results.Add(result);
    }

    private void Align(float targetOrientation)
    {
        SteeringOutput result = new SteeringOutput();
        if (isKinematic)
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
        results.Add(result);
    }

    float MapAngleToRange(float angle)
    {
        float r = angle % 360;

        if (r < -180) r += 360;
        if (r > 180) r -= 360;
        return r;
    }

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

    private void LookWhereYouGo()
    {
        if (isKinematic && output.velocity.magnitude > 0.2f) Align(Mathf.Atan2(output.velocity.x, output.velocity.z) * Mathf.Rad2Deg);
        else if (rb.velocity.magnitude > 0.2f) Align(Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg);

        return;
    }

    private void Pursue(Transform target_, float weight = 1)
    {
        targetRB = target_.GetComponent<Rigidbody>();
        Vector3 targetPos = target_.position;

        float prediction;
        float maxPrediction = maxSpeed;

        if (targetRB)
        {
            Vector3 dir = target_.position - transform.position;
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

            targetPos += weight * (targetRB.velocity * prediction);
        }

        Arrive(targetPos, weight);
    }

    private void Evade(Transform target_, float weight = 1)
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

            targetPos += weight * (targetRB.velocity * prediction);
        }

        Avoid(targetPos, weight);
    }

    public void ObstacleAvoidance(float weight = 3)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f + transform.forward * 0.8f, transform.forward);
        RaycastHit forwardHit;

        Vector3 leftWhiskerForward = Quaternion.AngleAxis(-whiskerAngle, Vector3.up) * transform.forward;
        Ray leftRay = new Ray(transform.position + Vector3.up * 0.5f + leftWhiskerForward * 0.8f, leftWhiskerForward);
        RaycastHit leftHit;

        Vector3 rightWhiskerForward = Quaternion.AngleAxis(whiskerAngle, Vector3.up) * transform.forward;
        Ray rightRay = new Ray(transform.position + Vector3.up * 0.5f + rightWhiskerForward * 0.8f, rightWhiskerForward);
        RaycastHit rightHit;

        // If the forward ray hits an obstacle
        if (Physics.Raycast(ray, out forwardHit, lookAhead))
        {
            //Debug.Log(forwardHit.transform.name);
            Vector3 hitNormal = forwardHit.normal;
            Vector3 hitPos = forwardHit.point;
            hitNormal.y = 0f;
            hitPos.y = 0f;

            Vector3 target;
            if (Vector3.Dot(ray.direction, hitNormal) < 0.9f)
            {
                target = hitPos + hitNormal * avoidDistance + Quaternion.AngleAxis(90.0f, Vector3.up) * ray.direction;
            }
            else
            {
                target = hitPos + hitNormal * avoidDistance + Quaternion.AngleAxis(-90.0f, Vector3.up) * ray.direction;
            }

            target.y = 0f;

            Vector3 toTargetPos = transform.position + new Vector3(0f, 0.5f, 0f);
            Vector3 toTargetDir = target - transform.position;

            Debug.DrawRay(toTargetPos, toTargetDir, Color.cyan, 1f);
            Seek(target, weight);
        }

        // If the left ray hits an obstacle
        if (Physics.Raycast(leftRay, out leftHit, lookAhead * 0.6f))
        {
            //Debug.Log(forwardHit.transform.name);
            Vector3 hitNormal = leftHit.normal;
            Vector3 hitPos = leftHit.point;
            hitNormal.y = 0f;
            hitPos.y = 0f;

            Vector3 target;

            if (Vector3.Dot(leftRay.direction, hitNormal) < 0.9f)
            {
                target = hitPos + hitNormal * avoidDistance + Quaternion.AngleAxis(90.0f, Vector3.up) * leftRay.direction;
            }
            else
            {
                target = hitPos + hitNormal * avoidDistance + Quaternion.AngleAxis(-90.0f, Vector3.up) * leftRay.direction;
            }


            target.y = 0f;

            Vector3 toTargetPos = transform.position + new Vector3(0f, 0.5f, 0f);
            Vector3 toTargetDir = target - transform.position;

            Debug.DrawRay(leftRay.origin, leftRay.direction, Color.red, 1f);
            Seek(target, weight);
        }

        // If the right ray hits an obstacle
        else if (Physics.Raycast(rightRay, out rightHit, lookAhead * 0.6f))
        {
            //Debug.Log(forwardHit.transform.name);
            Vector3 hitNormal = rightHit.normal;
            Vector3 hitPos = rightHit.point;
            hitNormal.y = 0f;
            hitPos.y = 0f;

            Vector3 target;

            if (Vector3.Dot(ray.direction, hitNormal) < 0.9f)
            {
                target = hitPos + hitNormal * avoidDistance + Quaternion.AngleAxis(-90.0f, Vector3.up) * rightRay.direction;
            }
            else
            {
                target = hitPos + hitNormal * avoidDistance + Quaternion.AngleAxis(90.0f, Vector3.up) * rightRay.direction;
            }


            target.y = 0f;

            Vector3 toTargetPos = transform.position + new Vector3(0f, 0.5f, 0f);
            Vector3 toTargetDir = target - transform.position;

            Debug.DrawRay(rightRay.origin, rightRay.direction, Color.red, 1f);
            Seek(target, weight);
        }


    }
}




        