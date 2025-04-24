using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public override void Enter()
    {
        base.Enter();

        // 巡逻状态计时器
        StateTimer = 10;

        // 设定动画参数
        animator.SetBool("isPatrolling", true);
    }

    public override void Exit()
    {
        base.Exit();

        // 设定动画参数
        animator.SetBool("isPatrolling", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        StateTimer -= Time.deltaTime;

        // 巡逻行为


        // 巡逻时间结束 切换至待机状态
        if(StateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
