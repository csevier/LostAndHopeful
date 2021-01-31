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

    [SyncVar]
    private float energy = 100.0f;

    public float sensitivity = 2.0f;
    public float smoothing = 5.0f;
    public float lookYLimit = 60.0f;
    public float lineOfSightDistance = 20.0f;

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

    private GameObject leftEye;
    private GameObject rightEye;
    private Text typeUI;
    private GameObject _hat;
    public Animator animator;

    public Texture hopefulTexture;
    public Texture lostTexture;
    public Texture lostTexture2;
    public Texture lostTextureHandsome;
    
    public ParticleSystem changeEffect;

    [SyncVar(hook=nameof(HandleTypeUpdate))] 
    [SerializeField] 
    public string type = "Hopeful";

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
        animator = GetComponent<Animator>();
        animator.Play("Idle");
        characterController = GetComponent<CharacterController>();
        _hat = gameObject.transform.Find("metarig/spine/spine.001/spine.002/hat").gameObject;
        leftEye = gameObject.transform.Find("PlayerCamera/leftEye").gameObject;
        rightEye = gameObject.transform.Find("PlayerCamera/rightEye").gameObject;
        typeUI = gameObject.transform.Find("PlayerCamera/UI/TypeUI").GetComponent<Text>();
        AdjustHat();
        movementSM = new StateMachine();
        standing = new StandingState(this, movementSM);
        jumping = new JumpingState(this, movementSM);
        falling = new FallingState(this, movementSM);
        movementSM.Initialize(falling);

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        //debug.text = movementSM.CurrentState.GetType().Name;
        DepleteEnergy();
        energySlider.value = energy;
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
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

    void DepleteEnergy()
    {
        // if (isAnnoying) return;
        //if (energy > 0.0)
        //{
        //      BecomeAnnoying()
        //}
        if (type == "Hopeful") return;
        energy -= 3.0f * Time.deltaTime;
    }

    public void OnOrbCollected()
    {
        if (type == "Hopeful") return;
        if (isServer)
        {
            energy += 20;
            // energy is sync to specific client called on server instance of player
            if (energy > 100.0f)
            {
                energy = 100.0f;
            }
        }
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
            if (type == "Lost")
            {
                // turn on enery ui and turn off lights
                var mat = GetComponent<SkinnedMeshRenderer>().materials[0];
                if (mat != null)
                {
                    mat.SetTexture("_MainTex", lostTexture);
                    mat.SetTexture("_EmissionMap", lostTexture);
                }
                typeUI.text = "You are: Lost";
                energySlider.gameObject.SetActive(true);
                leftEye.SetActive(false);
                rightEye.SetActive(false);
            }
            else
            {
                typeUI.text = "You are: Hopeful";
                var mat = GetComponent<SkinnedMeshRenderer>().materials[0];
                if (mat != null)
                {
                    mat.SetTexture("_MainTex", hopefulTexture);
                    mat.SetTexture("_EmissionMap", hopefulTexture);
                }
                energySlider.gameObject.SetActive(false);
                leftEye.SetActive(true);
                rightEye.SetActive(true);
            }

        }
    }

    private void HandleTypeUpdate(string oldType, string newType)
    {
        this.type = newType;
        if (oldType == "Lost" && newType == "Hopeful")
        {
            changeEffect.Play();
        }
        AdjustHat();
    }
    
    bool IsSeeingAnotherPlayer()
    {
        //lost dont look
        if (type == "Lost") return false;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.TransformDirection(Vector3.forward), out hit, lineOfSightDistance))
        {
            Character other = hit.collider.gameObject.GetComponent<Character>();
            if (other != null)
            {
                other.SetCharacterType("Hopeful");
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
