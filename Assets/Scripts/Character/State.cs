using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class State : MonoBehaviour
{
    protected Animator animator; 
    protected StateMachine stateMachine;

    protected float StateTimer;

    public virtual void Enter() { Debug.Log("Enter State: " + this.ToString()); }
    public virtual void HandleInput() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }
}
