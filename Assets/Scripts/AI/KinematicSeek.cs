using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(SteeringOutput))]
[RequireComponent(typeof(Rigidbody))]

public class KinematicSeek : MonoBehaviour
{
    SteeringOutput result;
    NPCMovement character;
    Rigidbody rb;

    [SerializeField, Tooltip("The target for the NPC")]
    Transform target;
    float maxSpeed;

    private void Awake()
    {
        if(!character)
        {
            character = GetComponent<NPCMovement>();
        }
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if(!result)
        {
            result = GetComponent<SteeringOutput>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void getSteering()
    {

        result.velocity = target.position - transform.position;

        result.velocity.Normalize();
        result.velocity *= maxSpeed;

        //rb.rotation.y = newOrientation()
    }

    float newOrientation()
    {
        Vector2 velocityXZ = new Vector2(rb.velocity.x, rb.velocity.z);
        if(velocityXZ.magnitude > 0)
        {
            return Mathf.Atan2(-rb.velocity.x, rb.velocity.z);
        }
        else
        {
            return transform.rotation.eulerAngles.y;
        }
        
    }
}
