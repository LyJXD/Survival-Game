using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Components
    [SerializeField]
    private Animator animator;
    public Animator Animator => animator;

    private EntityStats stats;
    public EntityStats Stats => stats;

    [SerializeField]
    private ParticleSystem HitPs;
    #endregion


    private void Awake()
    {
        stats = GetComponent<EntityStats>();
    }

    public virtual void DamageEffect()
    {
        HitPs.Play();

        Debug.Log(gameObject.name + " was damaged!");
    }

    public virtual void Death()
    {

    }
}
