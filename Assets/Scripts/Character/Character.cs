using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Enemy NPC
/// </summary>
public class Character : Entity
{
    [Header("Attack Check")]
    public Transform attackCheck;
    public float attackCheckRadius;

    public virtual void Attack()
    {

    }
}
