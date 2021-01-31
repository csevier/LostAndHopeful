using System;
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


    public float sensitivity = 2.0f;
    public float smoothing = 5.0f;
    public float lookYLimit = 60.0f;
    public float lineOfSightDistance = 20.0f;

    private Camera playerCam;
    private Transform childCameraTransform;
    private Vector2 mouseLook;
    private Vector2 smoothV;

    CharacterController characterController;
    public StateMachine movementSM;
    public StandingState standing;
    public JumpingState jumping;
    public FallingState falling;

    public Text debug;

    private GameObject _hat;
    private GameObject _eyes;
    [SyncVar(hook=nameof(HandleTypeUpdate))] 
    [SerializeField] 
    private string type = "Hopeful";

    void Awake()
    {
        playerCam = GetComponentInChildren<Camera>();
        playerCam.enabled = false;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        _hat = gameObject.transform.Find("hat").gameObject;
        _eyes = gameObject.transform.Find("PlayerCamera").gameObject;
        AdjustHat();
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
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        movementSM.CurrentState.PhysicsUpdate();
        MouseLook();
        IsSeeingAnotherPlayer();

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

    [Server]
    public void SetCharacterType(string type)
    {
        this.type = type;
    }

    void AdjustHat()
    {
        if (_hat != null)
        {
            _hat.SetActive(type == "Lost");
        }
    }

    private void HandleTypeUpdate(string oldType, string newType)
    {
        this.type = newType;
        AdjustHat();
    }
    
    bool IsSeeingAnotherPlayer()
    {
        //lost dont look
        if (type == "Lost") return false;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_eyes.transform.position, _eyes.transform.TransformDirection(Vector3.forward), out hit, lineOfSightDistance))
        {
            Character other = hit.collider.gameObject.GetComponent<Character>();
            if (other != null)
            {
                Debug.Log("Did Hit Player");
                other.SetCharacterType("Hopeful");
            }
            else
            {
                Debug.Log("Did Hit Object");
            }

            return true;
        }
        else
        {
            Debug.Log("Did not Hit Anything");
            return false;
        }
    }

}
