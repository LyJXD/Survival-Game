using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprintState : PlayerState
{

    bool grounded;
    bool sprint;
    bool sprintJump;

    float gravityValue;
    float playerSpeed;
    Vector3 currentVelocity;
    Vector3 cVelocity;


    public override void Enter()
    {
        base.Enter();

        sprint = false;
        sprintJump = false;

        input = Vector2.zero;
        velocity = Vector3.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = player.sprintSpeed;
        grounded = player.characterController.isGrounded;
        gravityValue = player.gravityValue;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        // 检测玩家输入
        input = moveAction.ReadValue<Vector2>();
        // 设定玩家速度
        velocity = new Vector3(input.x, 0, input.y);
        velocity = velocity.x * player.transform.right.normalized + velocity.z * player.transform.forward.normalized;
        velocity.y = 0f;

        // 检测状态切换
        if (sprintAction.triggered || input.sqrMagnitude == 0f)
        {
            sprint = false;
        }
        else
        {
            sprint = true;
        }
        if (jumpAction.triggered)
        {
            sprintJump = true;

        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 执行状态切换
        if (sprint)
        {
            // 设定动画参数
            animator.SetFloat("Speed", input.magnitude + 0.5f, player.speedDampTime, Time.deltaTime);
        }
        else
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (sprintJump)
        {
            stateMachine.ChangeState(player.sprintJumpState);
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

        // 执行玩家移动
        player.characterController.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(velocity), player.rotationDampTime);
        }
    }

    public override void Exit()
    {

    }
}
