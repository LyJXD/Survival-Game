using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerIdleState : PlayerState
{
    // ״̬�ж�Booleans
    bool grounded;
    bool sprint;
    bool jump;

    float playerSpeed;
    float gravityValue;

    Vector3 currentVelocity; 
    Vector3 cVelocity;

    public override void Enter()
    {
        base.Enter();

        jump = false;
        sprint = false;

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
