using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    [Header("Controls")]
    public new PlayerStats Stats;
    public float playerSpeed = 5.0f;
    public float sprintSpeed = 7.0f;
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2;
    public float rotationSpeed = 5f;
    public GameObject WeaponHolder;

    [Header("Attack Info")]
    public int comboCounter;
    public float comboWindow = 2f;  // 连击窗口，攻击间隔不超过该时间则计算连击
    private float lastTimeAttacked;
    protected InputAction attackAction;

    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 1)]
    public float velocityDampTime = 0.9f;
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
        attackAction = playerInput.actions["Attack"];

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
        if (attackAction.triggered && WeaponHolder.transform.childCount != 0)
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
    #endregion

    #region Player Arm Movement
    public override void Attack()
    {
        if (comboCounter > 1 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        // 执行攻击动画
        Animator.SetTrigger("Attack");
        Animator.SetInteger("ComboCounter", comboCounter);
    }
    #endregion

    public void AttackFinishTrigger()
    {
        comboCounter++;
        lastTimeAttacked = Time.time;
        WeaponHolder.GetComponentInChildren<Collider>().enabled = false;
    }
}

