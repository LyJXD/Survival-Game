using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public abstract class PlayerState :State
{
    protected Player player;

    #region ActionMap
    protected InputAction moveAction;
    protected InputAction jumpAction;
    protected InputAction sprintAction;
    #endregion
    
    protected Vector2 input;
    protected Vector3 velocity;
    protected Vector3 gravityVelocity;

    public void Setup(Animator _animator, Player _player, StateMachine _stateMachine)
    {
        animator = _animator;
        player = _player;
        stateMachine = _stateMachine;

        moveAction = player.playerInput.actions["Movement"];
        jumpAction = player.playerInput.actions["Jump"];
        sprintAction = player.playerInput.actions["Sprint"];
    }

}
