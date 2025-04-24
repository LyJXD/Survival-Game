using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    [SerializeField]
    private Animator _animator;
    public Animator Animator => _animator;
    [SerializeField]
    private EntityStatus _status;
    public EntityStatus Status => _status;
    #endregion

    public virtual void Death()
    {

    }
}
