using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float gravity = -9.81f;

    // Footsteps audio information
    AudioSource footstepsAudio;
    private float footstepPitch;

    AudioSource breathingAudio;
    private float breathingPitch;

    // Footsteps audio variables to calculate if the audio is supposed to play
    private float minMoveDistance = 0.01f;
    private Vector3 previousPosition;

    // Values for the direction of the movement
    private CharacterController cc;
    bool tutorialEnd = false;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 vel;

    [SerializeField] float sprintDuration = 3f;
    private float stamina = 1;
    private bool tired = false;
    private float startMoveSpeed;

    [Header("Ground Check")]
    [SerializeField] float playerHeight = 2f;
    [SerializeField] LayerMask whatIsGround;
    bool grounded;

    [Header("Interaction")]
    [SerializeField] Camera cam;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private Vector3 interactionRay = default;
    [SerializeField] private float interactionDist = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    private void Start()
    {
        cc = FindObjectOfType<CharacterController>();
        startMoveSpeed = moveSpeed;
        
        // All information needed for the footsteps audio
        footstepsAudio = transform.GetChild(2).GetComponent<AudioSource>();
        footstepPitch = footstepsAudio.pitch;
        previousPosition = transform.position;

        // Information for breathing audio
        breathingAudio = transform.GetChild(3).GetComponent<AudioSource>();
        breathingPitch = breathingAudio.pitch;
        breathingAudio.Stop();
    }

    private void Update()
    {
        // Calculate if the player moves, if he does then play the footsteps sound
        float moveDistance = Vector3.Distance(transform.position, previousPosition);
        if (moveDistance >= minMoveDistance)
        {
            // Play the audio if isnt playing, this will make it so that the audio does not restart all the time
            if(!footstepsAudio.isPlaying)
                footstepsAudio.Play();

            previousPosition = transform.position;
        }
        // If the player isnt moving the audio will not play
        else
            footstepsAudio.Stop();
        
        HandleSprint();
        HandleMovement();

        if (canInteract)
        {
            InteractionCheck();
            InteractionInput();
        }

        
    }

    private void InteractionCheck()
    {
        // Casts a ray and collides with anything
        if (Physics.Raycast(cam.ViewportPointToRay(interactionRay), out RaycastHit hit, interactionDist))
        {
            // Checks if ray is on the right layer and if theres a current interactable object hovered 
            if (hit.collider.gameObject.layer == 8 && (currentInteractable == null || currentInteractable.GetInstanceID() != hit.collider.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                    currentInteractable.OnHover();
            }
        }
        // Removes the current interactable if the player stopped looking at one
        else if (currentInteractable)
        {
            currentInteractable.OffHover();
            currentInteractable = null;
        }
    }

    private void InteractionInput()
    {
        // Runs OnInteract if player clicks on an interactable
        if (Input.GetButtonDown("Fire1") && currentInteractable != null && Physics.Raycast(cam.ViewportPointToRay(interactionRay), out RaycastHit hit, interactionDist, interactionLayer))
            currentInteractable.OnInteract();
    }

    private void HandleSprint()
    {
        // Checks if player is holding sprint button and has not exhausted all stamina
        if (Input.GetKey(KeyCode.LeftShift) && !tired)
        {
            breathingAudio.Play();

            // Change the pitch of the sound so it sounds like running
            if (footstepsAudio.pitch != 1.2)
                footstepsAudio.pitch = 1.2f;

            if (moveSpeed == startMoveSpeed)
                moveSpeed *= 1.3f;

            if (stamina > 0)
                stamina -= (1 / sprintDuration) * Time.deltaTime;
            else
            {
                breathingAudio.pitch = 1f;
                tired = true;
            }
        }
        // Lowers movement speed if player is exhausted and regenerates stamina
        else if (tired)
        {
            // Change the pitch of the sound so it sounds like the player is tired
            if (footstepsAudio.pitch != 0.6)
                footstepsAudio.pitch = 0.6f;

            if (moveSpeed != startMoveSpeed * 0.6f)
                moveSpeed = startMoveSpeed * 0.6f;

            if (stamina < 1)
                stamina += (1 / (sprintDuration * 2f)) * Time.deltaTime;
            else
            {
                breathingAudio.pitch = breathingPitch;
                breathingAudio.Stop();
                tired = false;
            }
        }
        // Resets movement speed if player is no longer sprinting and is not tired and regenerates stamina
        else
        {
            // Change the audio pitch to the original value
            if (footstepsAudio.pitch != footstepPitch)
                footstepsAudio.pitch = footstepPitch;

            if (moveSpeed != startMoveSpeed)
                moveSpeed = startMoveSpeed;

            if (stamina < 1)
                stamina += (1 / (sprintDuration * 2f)) * Time.deltaTime;
        }

    }

    private void HandleMovement()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
            vel.y = -2f;

        UserInput();

        moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;

        cc.Move(moveDirection * moveSpeed * Time.deltaTime);

        vel.y += gravity * Time.deltaTime;

        cc.Move(vel * Time.deltaTime);
        
    }

    private void UserInput()
    {
        // The movement from the player Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider col)
    {
        // The player has cleared the tutorial
        if (col.CompareTag("ToriExit"))
            tutorialEnd = true;
    }
    public bool GetTutorialState()
    {
        // The enemy checks if its okey to chase the player
        return tutorialEnd;
    }
}
