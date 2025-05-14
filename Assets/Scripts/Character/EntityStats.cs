using System;
using UnityEngine;

/// <summary>
/// ����ʵ�� - ��� �ж����� NPC �� ���� �� - Status
/// </summary>
public class EntityStats : MonoBehaviour
{
    #region Stats
    public Stats maxHealth;            // ����ֵ
    public Stats attack;               // ������
    public Stats defense;              // ������
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
    public virtual void DoDamage(EntityStats targetStatus)
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