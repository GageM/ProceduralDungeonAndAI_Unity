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
    LOOK_WHERE_MOVING
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

    [SerializeField, Tooltip("The Y position to check for obstacles")]
    float whiskerHeight = 0.9f;

    [SerializeField, Tooltip("The 'Whisker' angle")]
    float whiskerAngle = 90f;

    [SerializeField, Tooltip("The Weighting of the Obstacle Avoidance Steering")]
    float obstacleAvoidanceWeight = 3.0f;

    [SerializeField, Tooltip("Controls How The Distance From An Object Weights Steering")]
    float obstacleDistanceWeightMultiplier = 1.0f;


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

    public bool GetMovementSteering(MoveState moveState_, Transform target_)
    {
        // Controls The Movement Steering Of The NPC
        switch (moveState_)
        {
            case MoveState.SEEK:
                Seek(target_.position);
                return false;
            case MoveState.FLEE:
                Flee(target_.position);
                return false;
            case MoveState.ARRIVE:
                return Arrive(target_.position);
            case MoveState.AVOID:
                return Avoid(target_.position);
            case MoveState.PURSUE:
                return Pursue(target_);
            case MoveState.EVADE:
                return Evade(target_);
            case MoveState.NONE:
                return false;
            default:
                return false;
        }
    }

    public void GetLookSteering(LookState lookState_, Transform target_)
    {
        if (!target_)
        {
            LookWhereYouGo();
            return;
        }

        switch (lookState_)
        {
            case LookState.LOOK_WHERE_MOVING:
                LookWhereYouGo();
                return;

            case LookState.LOOK_AT_TARGET:
                LookAtTarget(target_.position);
                return;

            case LookState.NONE:
                return;

            default:
                return;
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
            DrawSteeringRay(result);
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

    private bool Arrive(Vector3 targetPos, float weight = 1.0f)
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
                return true;
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
                return true;
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
        return false;
    }

    private bool Avoid(Vector3 targetPos, float weight = 1.0f)
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
                return true;
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
                return true;
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
        return false;
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

    private bool Pursue(Transform target_, float weight = 1)
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

        return Arrive(targetPos, weight);
    }

    private bool Evade(Transform target_, float weight = 1)
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

        return Avoid(targetPos, weight);
    }

    public void ObstacleAvoidance(float weight = 3f)
    {
        Ray ray = new(transform.position + Vector3.up * whiskerHeight + transform.forward * 0.8f, transform.forward);
        RaycastHit forwardHit;

        List<RaycastHit> hits = new();

        Vector3 leftWhiskerForward = Quaternion.AngleAxis(-whiskerAngle, Vector3.up) * transform.forward;
        Ray leftRay = new(transform.position + Vector3.up * whiskerHeight + leftWhiskerForward * 0.8f, leftWhiskerForward);
        RaycastHit leftHit;

        Vector3 rightWhiskerForward = Quaternion.AngleAxis(whiskerAngle, Vector3.up) * transform.forward;
        Ray rightRay = new(transform.position + Vector3.up * whiskerHeight + rightWhiskerForward * 0.8f, rightWhiskerForward);
        RaycastHit rightHit;

        List<Vector3> targets = new();


        // Check if any of the three rays hit an obstacle
        if (Physics.Raycast(ray, out forwardHit, lookAhead * Mathf.Sin(whiskerAngle * Mathf.Deg2Rad)))
        {
            //Debug.Log(forwardHit.transform.name);
            Vector3 hitNormal = forwardHit.normal;
            Vector3 hitPos = forwardHit.point;
            hitNormal.y = 0f;
            hitPos.y = 0f;

            hits.Add(forwardHit);

            Vector3 target;
            target = hitPos + hitNormal * avoidDistance;

            target.y = 0f;

            targets.Add(target);

            Debug.DrawRay(ray.origin, transform.forward * avoidDistance * Mathf.Sin(whiskerAngle * Mathf.Deg2Rad), Color.red);
        }

        if (Physics.Raycast(leftRay, out leftHit, lookAhead * Mathf.Sin(whiskerAngle * Mathf.Deg2Rad)))
        {
            //Debug.Log(forwardHit.transform.name);
            Vector3 hitNormal = leftHit.normal;
            Vector3 hitPos = leftHit.point;
            hitNormal.y = 0f;
            hitPos.y = 0f;

            hits.Add(leftHit);

            Vector3 target;

            target =  rightRay.origin + (leftHit.distance * rightWhiskerForward);

            target += hitNormal * avoidDistance - transform.forward * avoidDistance;

            target.y = 0f;

            targets.Add(target);

            Debug.DrawRay(leftRay.origin, leftWhiskerForward * avoidDistance * Mathf.Sin(whiskerAngle * Mathf.Deg2Rad), Color.red);
        }

        else if (Physics.Raycast(rightRay, out rightHit, lookAhead * Mathf.Sin(whiskerAngle * Mathf.Deg2Rad)))
        {
            //Debug.Log(forwardHit.transform.name);
            Vector3 hitNormal = rightHit.normal;
            Vector3 hitPos = rightHit.point;
            hitNormal.y = 0f;
            hitPos.y = 0f;

            hits.Add(rightHit);

            Vector3 target;

            target = leftRay.origin + (rightHit.distance * leftWhiskerForward);

            target += hitNormal * avoidDistance - transform.forward * avoidDistance;

            target.y = 0f;

            targets.Add(target);

            Debug.DrawRay(rightRay.origin, rightWhiskerForward * avoidDistance * Mathf.Sin(whiskerAngle * Mathf.Deg2Rad), Color.red);
        }

        // If a ray hit an obstacle seek the average of targets
        if(targets.Count > 0)
        {
            Vector3 seekTarget = targets[targets.Count - 1];
            RaycastHit seekHit = hits[targets.Count - 1];


            Seek(seekTarget, obstacleAvoidanceWeight / (seekHit.distance * obstacleDistanceWeightMultiplier));
        }
    }

    void DrawSteeringRay(SteeringOutput result)
    {
        if (isKinematic)
        {
            Debug.DrawRay(transform.position, result.velocity * result.weight, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, result.linearAcceleration * result.weight, Color.yellow);        
        }
    }
}




        