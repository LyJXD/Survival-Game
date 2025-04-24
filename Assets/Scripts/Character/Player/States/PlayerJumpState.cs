using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerJumpState : PlayerState
{
    bool grounded;

    float gravityValue;
    float jumpHeight;
    float playerSpeed;

    Vector3 airVelocity;
    public override void Enter()
    {
        base.Enter();

        grounded = false;
        gravityValue = player.gravityValue;
        jumpHeight = player.jumpHeight;
        playerSpeed = player.playerSpeed;
        gravityVelocity.y = 0;

        // 设定动画参数
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Jump");
        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = moveAction.ReadValue<Vector2>();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 玩家落地 切换至着陆状态
        if (grounded)
        {
            stateMachine.ChangeState(player.landState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!grounded)
        {
            velocity = player.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);

            velocity = velocity.x * player.transform.right.normalized + velocity.z * player.transform.forward.normalized;
            velocity.y = 0f;
            airVelocity = airVelocity.x * player.transform.right.normalized + airVelocity.z * player.transform.right.normalized;
            airVelocity.y = 0f;
            player.characterController.Move(gravityVelocity * Time.deltaTime + (airVelocity * player.airControl + velocity * (1 - player.airControl)) * playerSpeed * Time.deltaTime);
        }

        gravityVelocity.y += gravityValue * Time.deltaTime;
        grounded = player.characterController.isGrounded;
    }

    public override void Exit()
    {

    }

    private void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}
