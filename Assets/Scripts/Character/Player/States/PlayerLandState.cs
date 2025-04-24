using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerLandState : PlayerState
{
    public override void Enter()
    {
        base.Enter();

        // ��½״̬��ʱ��
        StateTimer = 0.5f;

        animator.SetTrigger("Land");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        StateTimer -= Time.deltaTime;

        // ��½ʱ����� �л�������״̬
        if (StateTimer < 0)
        {
            animator.SetTrigger("Move");
            stateMachine.ChangeState(player.idleState);
        }
    }
}
