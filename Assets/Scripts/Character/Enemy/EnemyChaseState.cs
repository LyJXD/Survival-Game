using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public override void Enter()
    {
        base.Enter();

        // �趨��������
        animator.SetBool("isChasing", true);
    }

    public override void Exit()
    {
        base.Exit();        
        
        // �趨��������
        animator.SetBool("isChasing", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ��⵽��� ִ��׷����Ϊ
        if (enemy.IsPlayerDetected)
        {
            enemy.agent.SetDestination(player.transform.position);
        }
        // ����߳�׷��Χ ׷��������������ڷ�Χ�ڵ�λ��
        else
        {
            enemy.agent.SetDestination(enemy.LastDetectPlayerPos);
        }
        // ����ڹ�����Χ�� �л�������״̬
        if (enemy.IsPlayerInAttackRange)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        // ׷��������������ڷ�Χ�ڵ�λ����Ȼδ��⵽��� �л���Ѳ��״̬
        if (!enemy.IsPlayerDetected && enemy.transform.position == enemy.LastDetectPlayerPos)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
}
