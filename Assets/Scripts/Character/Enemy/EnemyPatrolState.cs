using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public override void Enter()
    {
        base.Enter();

        // Ѳ��״̬��ʱ��
        StateTimer = 10;

        // �趨��������
        animator.SetBool("isPatrolling", true);
    }

    public override void Exit()
    {
        base.Exit();

        // �趨��������
        animator.SetBool("isPatrolling", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        StateTimer -= Time.deltaTime;

        // Ѳ����Ϊ


        // Ѳ��ʱ����� �л�������״̬
        if(StateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
