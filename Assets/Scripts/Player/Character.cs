using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    GameManager gameManager;
    Rigidbody rb;
    Animator animator;

    [Header("Character Movement")]
    [SerializeField, Tooltip("How fast the character can Walk")]
    public float walkSpeed;
    [SerializeField, Tooltip("How fast the character can Run")]
    public float runSpeed;
    [SerializeField, Tooltip("The speed at which the character jumps")]
    float jumpSpeed;

    //[HideInInspector]
    public float currentSpeed;
    //[HideInInspector]
    public float moveForward;
    //[HideInInspector]
    public float moveSide;

    public bool disableInput = false;

    [Header("Ground Checking")]
    [SerializeField, Tooltip("Is the character on the ground")]
    bool isGrounded;
    [SerializeField, Tooltip("Ground Level")]
    Transform groundCheck;
    [SerializeField, Tooltip("Maximum distance from ground to be considered 'grounded'")]
    float groundCheckDistance;

    [Header("Combat Stats")]
    [SerializeField, Tooltip("The amount of health the player has")]
    float health;


    // The Ground Layer is layer 3
    int groundLayerMask = 1 << 3;
    // The Enemy Layer is Layer 6
    int enemyLayerMask = 1 << 6;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!rb)
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Initialize unset variables
        if(walkSpeed <= 0)
        {
            walkSpeed = 5f;

            Debug.Log(name + ": walkSpeed not set, defaulting to " + walkSpeed);
        }

        if (runSpeed <= 0)
        {
            runSpeed = 8f;

            Debug.Log(name + ": runSpeed not set, defaulting to " + runSpeed);
        }

        if (jumpSpeed <= 0)
        {
            runSpeed = 8f;

            Debug.Log(name + ": jumpSpeed not set, defaulting to " + jumpSpeed);
        }

        if (!groundCheck)
        {
            Debug.LogError(name + ": missing groundCheck");
        }

        if (groundCheckDistance <= 0)
        {
            groundCheckDistance = 0.3f;

            Debug.Log(name + ": groundCheckDistance not set, defaulting to " + groundCheckDistance);
        }

        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (groundCheck)
        {
            isGrounded = Physics.Raycast(groundCheck.position - groundCheck.up * -0.001f, -groundCheck.up, groundCheckDistance);

            Debug.DrawRay(groundCheck.position, -groundCheck.up * groundCheckDistance, Color.cyan);
        }

        if (!disableInput)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                gameManager.SetGameState(GameState.GameOver);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (currentSpeed != runSpeed)
                {
                    currentSpeed = runSpeed;
                }
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            moveForward = Input.GetAxis("Vertical");
            moveSide = Input.GetAxis("Horizontal");

            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            animator.SetFloat("Y", moveForward);
            animator.SetFloat("X", moveSide);
        }
    }


    void FixedUpdate()
    {
            rb.velocity = (transform.forward * moveForward * currentSpeed) + (transform.right * moveSide * currentSpeed) + (transform.up * rb.velocity.y);
    }

    public void Jump()
    {
        Debug.Log("Jumping");

        rb.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
    }

}