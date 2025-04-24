using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    [Header("Controls")]
    public float playerSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2;
    public float rotationSpeed = 5f;
    public GameObject ToolHolder;

    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 1)]
    public float velocityDampTime = 0.9f;
    [Range(0, 1)]
    public float rotationDampTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public bool isGrounded;

    [Header("Object Check")]
    public Transform objectCheck;
    public float accessRange = 2f;
    public bool isObjectCanInteract; 

    [HideInInspector]
    public CharacterController characterController; 
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public float gravityValue = -9.81f;
    [HideInInspector]
    public Vector3 playerVelocity;

    #region PlayerState
    public StateMachine playerStateMachine;
    public PlayerIdleState idleState;
    public PlayerSprintState sprintState;
    public PlayerSprintJumpState sprintJumpState;
    public PlayerJumpState jumpState;
    public PlayerLandState landState;
    #endregion

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        playerStateMachine = new StateMachine();
        idleState.Setup(Animator, this, playerStateMachine);
        sprintState.Setup(Animator, this, playerStateMachine);
        sprintJumpState.Setup(Animator, this, playerStateMachine);
        jumpState.Setup(Animator, this, playerStateMachine);
        landState.Setup(Animator, this, playerStateMachine);

        // 初始化角色状态
        playerStateMachine.Initialize(idleState); 
        
        gravityValue *= gravityMultiplier;
    }

    private void Update()
    {   
        GroundCheck();
        ObjectCheck();

        playerStateMachine.currentState.HandleInput();
        playerStateMachine.currentState.LogicUpdate();

        // 玩家按下左键攻击
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        playerStateMachine.currentState.PhysicsUpdate();
    }

    #region Collision Checks
    private void GroundCheck() => isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    private void ObjectCheck() => isObjectCanInteract = Physics.CheckSphere(objectCheck.position, accessRange);
    private int DistanceToSelectedObject()
    {
        return 0;
    }
    #endregion

    #region Player Arm Movement
    public override void Attack()
    {
        // 执行攻击动画

        // 检测到生物/树/矿物 并且手上是工具 在工具使用范围内 就可以对生物/树/矿物造成伤害 - 攻击/砍树/采矿
        if (DistanceToSelectedObject() < attackCheckRadius && ToolHolder.GetComponentInChildren<ToolItem>())
        {
            Status.DoDamage(SelectionManager.Instance.SelectedObjectStatus);
        }
    }
    #endregion

}

