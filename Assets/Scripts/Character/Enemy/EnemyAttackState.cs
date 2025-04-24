using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public override void Enter()
    {
        base.Enter();

        // �趨��������
        animator.SetBool("isAttacking", true);
    }

    public override void Exit()
    {
        base.Exit();        
        
        // �趨��������
        animator.SetBool("isAttacking", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ����ڹ�����Χ�� ִ�й���
        if (enemy.IsPlayerInAttackRange)
        {

        }
        // ����߳�������Χ �л���׷��״̬
        else
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
