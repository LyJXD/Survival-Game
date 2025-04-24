using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public override void Enter()
    {
        base.Enter();

        // 设定动画参数
        animator.SetBool("isAttacking", true);
    }

    public override void Exit()
    {
        base.Exit();        
        
        // 设定动画参数
        animator.SetBool("isAttacking", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 玩家在攻击范围内 执行攻击
        if (enemy.IsPlayerInAttackRange)
        {

        }
        // 玩家走出攻击范围 切换至追逐状态
        else
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
