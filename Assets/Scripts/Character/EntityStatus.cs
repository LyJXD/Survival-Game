using System;
using UnityEngine;

/// <summary>
/// ����ʵ�� - ��� �ж����� NPC �� ���� �� - Status
/// </summary>
public class EntityStatus : MonoBehaviour
{
    #region Status
    public Status maxHealth;            // ����ֵ
    public Status attack;               // ������
    public Status defense;              // ������
    #endregion

    public int CurrentHealth { get; protected set; }

    public event Action OnHealthReachedZero;

    public virtual void Awake()
    {
        // ���ó�ʼ����ֵ
        CurrentHealth = maxHealth.GetValue();
    }

    public virtual void Start()
    {

    }

    /// <summary>
    /// ��Ŀ������˺�
    /// </summary>
    public virtual void DoDamage(EntityStatus targetStatus)
    {
        // ���������˺���ȷ���˺�ֵ�������0
        int totalDamage = attack.GetValue() - targetStatus.defense.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        targetStatus.TakeDamage(totalDamage);
    }

    /// <summary>
    /// �ܵ��˺�
    /// </summary>
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // ������ֵ���� DIE
        if (CurrentHealth <= 0)
        {
            OnHealthReachedZero?.Invoke();
        }
    }

    /// <summary>
    /// �ظ�����ֵ
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth.GetValue());
    }
}