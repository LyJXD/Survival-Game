using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Enemy : Character
{
    [Header("Detect Settings")]
    public NavMeshAgent agent;
    public Player player;
    [Range(0, 360)]
    public float detectAngle = 270f;  // 视野角度
    [SerializeField]
    private float detectRadius; 
    [SerializeField]
    private int detectAngleStep = 20; // 射线密度
    private List<Ray> detectRay;

    public Vector3 LastDetectPlayerPos { get; private set; }
    public float DistanceToPlayer { get; private set; }
    public bool IsPlayerDetected { get; private set; }
    public bool IsPlayerInAttackRange { get; private set; }

    #region EnemyState
    public StateMachine enemyStateMachine;
    public EnemyIdleState idleState;
    public EnemyChaseState chaseState;
    public EnemyPatrolState patrolState;
    public EnemyAttackState attackState;
    #endregion

    private void Start()
    {
        enemyStateMachine = new StateMachine();
        idleState.Setup(Animator, this, enemyStateMachine);
        chaseState.Setup(Animator, this, enemyStateMachine);
        patrolState.Setup(Animator, this, enemyStateMachine);
        attackState.Setup(Animator, this, enemyStateMachine);

        // 初始化敌人状态
        enemyStateMachine.Initialize(idleState);
    }

    private void Update()
    {
        CreateDetectRay();
        CheckIfPlayerIsDetected();
        if(IsPlayerDetected)
        {
            LastDetectPlayerPos = player.transform.position;
            CheckDistanceToPlayer();
            CheckIfPlayerCanBeAttacked();
        }

        enemyStateMachine.currentState.HandleInput();
        enemyStateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        enemyStateMachine.currentState.PhysicsUpdate();
    }

    // 检测玩家相关
    #region Detect Player
    private void CreateDetectRay()
    {
        // 计算最左侧方向的向量
        Vector3 forward_left = Quaternion.Euler(0, -(detectAngle / 2f), 0) * transform.forward;

        for (int i = 0; i <= detectAngleStep; i++)
        {
            // 根据当前角度计算方向向量
            Vector3 v = Quaternion.Euler(0, (detectAngle / detectAngleStep) * i, 0) * forward_left;

            // 添加侦测射线
            detectRay.Add(new Ray(transform.position, v));
        }

        // 绘制射线范围
        Gizmos.color = Color.red;
        foreach (var ray in detectRay)
        {
            Gizmos.DrawLine(transform.position, ray.origin + ray.direction * detectRadius);
        }
    }

    private void CheckIfPlayerIsDetected()
    {
        if (IsPlayerInAttackRange)
        {
            IsPlayerDetected = true;
            return;
        }

        foreach (var ray in detectRay)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, detectRadius) && hit.collider.CompareTag("Player"))
            {
                IsPlayerDetected = true;
                return;
            }
            else
            {
                IsPlayerDetected = false;
            }
        }
    }
    
    private void CheckDistanceToPlayer() => DistanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
    private void CheckIfPlayerCanBeAttacked() => IsPlayerInAttackRange = DistanceToPlayer < attackCheckRadius;
    #endregion

    private void OnDrawGizmos()
    {
        // 绘制攻击范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackCheck.transform.position, attackCheckRadius);
    }
}
