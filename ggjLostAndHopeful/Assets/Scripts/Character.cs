using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(CharacterController))]

public class Character : NetworkBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public Vector3 moveDirection = Vector3.zero;
    private float groundDistance = 1.1f;

    [SyncVar]
    private float energy = 100.0f;


    public float sensitivity = 2.0f;
    public float smoothing = 5.0f;
    public float lookYLimit = 60.0f;

    private Camera playerCam;
    private AudioListener playerListener;
    private Slider energySlider;
    private Transform childCameraTransform;
    private Vector2 mouseLook;
    private Vector2 smoothV;



    CharacterController characterController;
    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public FallingState falling;

    public Text debug;

    void Awake()
    {
        playerCam = GetComponentInChildren<Camera>();
        playerListener = GetComponentInChildren<AudioListener>();
        // may need to be more specific if there are more sliders in the future
        energySlider = GetComponentInChildren<Slider>(); 
        playerCam.enabled = false;
        playerListener.enabled = false;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
        falling = new FallingState(this, movementSM);
        movementSM.Initialize(falling);

        // Lock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        playerCam.enabled = true;
        playerListener.enabled = true;

    }

    public bool IsGrounded()
    {
        
        //bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance);
        return characterController.isGrounded;
    }
    
    public void Move(Vector3 moveDirection)
    {
        if (!isLocalPlayer) return;
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        movementSM.CurrentState.HandleInput();
        movementSM.CurrentState.LogicUpdate();
        debug.text = movementSM.CurrentState.GetType().Name;
        DepleteEnergy();
        energySlider.value = energy;
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        movementSM.CurrentState.PhysicsUpdate();
        MouseLook();
    }

    void MouseLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);

        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -lookYLimit, lookYLimit);

        playerCam.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
    }

    void DepleteEnergy()
    {
        // if (isAnnoying) return;
        //if (energy > 0.0)
        //{
        //      BecomeAnnoying()
        //}
        energy -= 3.0f * Time.deltaTime;
    }

    public void OnOrbCollected()
    {
        if (isServer) {
            energy += 20;
            // energy is sync to specific client called on server instance of player
            if (energy > 100.0f)
                energy = 100.0f;
        }
    }

}
