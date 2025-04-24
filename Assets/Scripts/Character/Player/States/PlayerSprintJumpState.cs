using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerSprintJumpState : PlayerState
{
    float timePassed;
    float jumpTime;


    public override void Enter()
    {
        base.Enter();

        animator.applyRootMotion = true;
        timePassed = 0f;
        animator.SetTrigger("SprintJump");

        jumpTime = 1f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // �趨�������� ִ��״̬�л�
        if (timePassed > jumpTime)
        {
            animator.SetTrigger("Move");
            stateMachine.ChangeState(player.sprintState);
        }
        timePassed += Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        animator.applyRootMotion = false;
    }
}
