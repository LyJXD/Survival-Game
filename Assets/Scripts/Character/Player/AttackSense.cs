using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSense : MonoBehaviour
{
    public Animator atkAnim;
    public AnimatorStateInfo animInfo;
    public float atkAnimSpeed;
    public float stopTime;
    public bool slowSpeed;


    private void Start()
    {

    }

    private void Update()
    {
        animInfo = atkAnim.GetCurrentAnimatorStateInfo(0);
    }

    public void OnAttack(Collider collider)
    {
        if (collider.GetComponent<Entity>() != null && animInfo.normalizedTime < 0.5f)
        {
            if (slowSpeed)
            {
                atkAnim.SetFloat("Speed", atkAnimSpeed);
                Invoke("resetSpeed", stopTime);
            }
        }
    }


    void resetSpeed()
    {
        atkAnim.SetFloat("Speed", 1f);
    }
    
}
