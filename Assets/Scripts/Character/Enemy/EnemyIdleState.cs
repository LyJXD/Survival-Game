using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ��⵽��� �л���׷��״̬
        if (enemy.IsPlayerDetected)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
