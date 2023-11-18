using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    // Dependancies
    Rigidbody rb;
    Camera mainCam;

    float currentStamina;
    public float CurrentStamina { 
        get { return currentStamina; }
        set { currentStamina = value; } 
    }

    // Input Information

    Vector3 move;
    bool disableInput = false;

    [Header("Movement Axes")]

    [SerializeField, Tooltip("Whether movement will follow the local or gobal world axes")]
    bool useCustomMovementAxes = false;

    [SerializeField, Tooltip("The rotation away from global axes on the Y axis for movement")]
    Vector3 CustomMovementAxes = Vector3.zero;

    // Movement

    [Header("Character Movement")]
    [SerializeField, Tooltip("How fast the character can Walk")]
    public float maxWalkSpeed;
    [SerializeField, Tooltip("How fast the character can Run")]
    public float maxRunSpeed;
    [SerializeField, Tooltip("How fast the character accelerates")]
    public float maxAcceleration;

    [SerializeField, Tooltip("The speed at which the character jumps")]
    float jumpHeight;

    float maxSpeed;

    bool isSprinting;

    [Header("Ground Checking")]
    bool isGrounded;
    [SerializeField, Tooltip("Ground Level")]
    Transform groundCheck;
    [SerializeField, Tooltip("Maximum distance from ground to be considered 'grounded'")]
    float groundCheckDistance;

    [Header("Interaction")]
    [SerializeField]
    float maxInteractionDistance = 10.0f;
    Transform interactOrigin;
    GameObject interactTarget;

    [Space(20)]
    // Unity Events
    [Header("Events")]
    [SerializeField]
    UnityEvent OnSprint = new();
    [SerializeField]
    UnityEvent OnJump  = new();
    [SerializeField]
    UnityEvent OnInteract = new();

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if(!rb)
        {
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if(!interactOrigin)
        {
            interactOrigin = transform.Find("SpringArm");
        }

        if(!mainCam)
        {
            mainCam = Camera.main;
        }

        // Initialize unset variables
        if(maxWalkSpeed <= 0)
        {
            maxWalkSpeed = 5f;

            Debug.Log(name + ": maxWalkSpeed not set, defaulting to " + maxWalkSpeed);
        }

        if (maxRunSpeed <= 0)
        {
            maxRunSpeed = 8f;

            Debug.Log(name + ": maxRunSpeed not set, defaulting to " + maxRunSpeed);
        }

        maxSpeed = maxWalkSpeed;

        if (maxAcceleration <= 0)
        {
            maxAcceleration = 15f;

            Debug.Log(name + ": maxAcceleration not set, defaulting to " + maxAcceleration);
        }

        if (jumpHeight <= 0)
        {
            jumpHeight = 8f;

            Debug.Log(name + ": jumpHeight not set, defaulting to " + jumpHeight);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (groundCheck)
        {
            isGrounded = Physics.Raycast(groundCheck.position - groundCheck.up * -0.001f, -groundCheck.up, groundCheckDistance);

            Debug.DrawRay(groundCheck.position, -groundCheck.up * groundCheckDistance, Color.cyan);
        }
        CheckForInteractable();
    }

    void FixedUpdate()
    {
        if (!disableInput)
        {
            Vector3 localMove = (transform.forward * move.z) + (transform.right * move.x);

            Vector3 globalMove = (Vector3.forward * move.z) + (Vector3.right * move.x);
            globalMove = Quaternion.Euler(CustomMovementAxes) * globalMove;

            // Accelerate character
            if(useCustomMovementAxes)
            {
                rb.AddForce(globalMove * maxAcceleration, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(localMove * maxAcceleration, ForceMode.Acceleration);
            }
            

            // Clamp max speed
            if(rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        move.x = context.ReadValue<Vector2>().x;
        move.z = context.ReadValue<Vector2>().y;
    }

    public void MoveWithMouse(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            move.z = 1.0f;
        }

        if(context.canceled)
        {
            move.z = 0.0f;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!disableInput)
        {
            if (isGrounded)
            {
                OnJump.Invoke();
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
            }
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (!disableInput)
        {
            if (context.performed)
            {
                if (currentStamina > 0)
                {
                    maxSpeed = maxRunSpeed;
                    isSprinting = true;
                    StartCoroutine(SprintUseStamina());
                }
                else
                {
                    maxSpeed = maxWalkSpeed;
                    isSprinting = false;
                }
            }
            if (context.canceled)
            {
                maxSpeed = maxWalkSpeed;
                isSprinting = false;
            }
        }
    }

    public void Quit(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        // Raycast to get mouse selection
        CheckForInteractable();

        // Interact with selection if it exists and can be interacted with
        if(interactTarget && interactTarget.TryGetComponent(out I_Interactable interactable))
        {
            interactable.Interact();
        }
        OnInteract.Invoke();
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        disableInput = !disableInput;
        if(disableInput)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    IEnumerator SprintUseStamina()
    {
        OnSprint.Invoke();
        while (isSprinting && currentStamina > 0.0f)
        {
            yield return new WaitForSeconds(0.05f);
            OnSprint.Invoke();
        }
        isSprinting = false;
        maxSpeed = maxWalkSpeed;
        yield break;
    }

    void CheckForInteractable()
    {
        // Get mouse cursor position
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCam.nearClipPlane;

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mousePos.z));

        RaycastHit hit;
        Physics.Raycast(new Ray(mouseWorld, mainCam.transform.forward), out hit);

        interactTarget = hit.transform.gameObject;

        // Check if the target is close enough to be interacted with
        if(Vector3.Magnitude(interactTarget.transform.position - transform.position) > maxInteractionDistance)
        {
            interactTarget = null;
        }
    }
}