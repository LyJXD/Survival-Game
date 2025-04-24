using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public override void Enter()
    {
        base.Enter();

        // 设定动画参数
        animator.SetBool("isChasing", true);
    }

    public override void Exit()
    {
        base.Exit();        
        
        // 设定动画参数
        animator.SetBool("isChasing", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 检测到玩家 执行追逐行为
        if (enemy.IsPlayerDetected)
        {
            enemy.agent.SetDestination(player.transform.position);
        }
        // 玩家走出追逐范围 追逐至玩家最后出现在范围内的位置
        else
        {
            enemy.agent.SetDestination(enemy.LastDetectPlayerPos);
        }
        // 玩家在攻击范围内 切换至攻击状态
        if (enemy.IsPlayerInAttackRange)
        {
            stateMachine.ChangeState(enemy.attackState);
        }
        // 追逐至玩家最后出现在范围内的位置依然未检测到玩家 切换至巡逻状态
        if (!enemy.IsPlayerDetected && enemy.transform.position == enemy.LastDetectPlayerPos)
        {
            stateMachine.ChangeState(enemy.patrolState);
        }
    }
}
