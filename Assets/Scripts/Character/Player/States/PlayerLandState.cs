using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerLandState : PlayerState
{
    public override void Enter()
    {
        base.Enter();

        // 着陆状态计时器
        StateTimer = 0.5f;

        animator.SetTrigger("Land");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        StateTimer -= Time.deltaTime;

        // 着陆时间结束 切换至待机状态
        if (StateTimer < 0)
        {
            animator.SetTrigger("Move");
            stateMachine.ChangeState(player.idleState);
        }
    }
}
