using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    float gravityValue;
    bool jump;
    Vector3 currentVelocity;
    bool grounded;
    bool sprint;
    float playerSpeed;

    Vector3 cVelocity;

    public override void Enter()
    {
        base.Enter();

        jump = false;
        sprint = false;
        input = Vector2.zero;

        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        velocity = player.playerVelocity;
        playerSpeed = player.playerSpeed;
        grounded = player.characterController.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        // ���״̬�л�
        if (jumpAction.triggered)
        {
            jump = true;
        }
        if (sprintAction.triggered)
        {
            sprint = true;
        }

        // ����������
        input = moveAction.ReadValue<Vector2>();
        // �趨����ٶ�
        velocity = new Vector3(input.x, 0, input.y);
        velocity = velocity.x * player.transform.right.normalized + velocity.z * player.transform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // �趨��������
        animator.SetFloat("Speed", input.magnitude, player.speedDampTime, Time.deltaTime);

        // ִ��״̬�л�
        if (sprint)
        {
            stateMachine.ChangeState(player.sprintState);
        }
        if (jump)
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = player.characterController.isGrounded;
        if (grounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }
        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref cVelocity, player.velocityDampTime);

        // ִ������ƶ�
        player.characterController.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity), player.rotationDampTime);
        }
    }
    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
