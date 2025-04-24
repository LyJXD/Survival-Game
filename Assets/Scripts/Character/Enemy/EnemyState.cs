using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : State
{
    protected Enemy enemy;
    protected Player player;

    public void Setup(Animator _animator, Enemy _enemy, StateMachine _stateMachine)
    {
        animator = _animator;
        enemy = _enemy;
        stateMachine = _stateMachine;

        player = enemy.player;
    }
}
