using System;
using UnityEngine;

/// <summary>
/// 所有实体 - 玩家 敌对生物 NPC 树 矿物 等 - Status
/// </summary>
public class EntityStats : MonoBehaviour
{
    #region Stats
    public Stats maxHealth;            // 生命值
    public Stats attack;               // 攻击力
    public Stats defense;              // 防御力
    #endregion

    public int CurrentHealth { get; protected set; }

    public event Action OnHealthReachedZero;

    public virtual void Awake()
    {
        // 设置初始生命值
        CurrentHealth = maxHealth.GetValue();
    }

    public virtual void Start()
    {

    }

    /// <summary>
    /// 对目标造成伤害
    /// </summary>
    public virtual void DoDamage(EntityStats targetStatus)
    {
        // 计算最终伤害并确保伤害值不会低于0
        int totalDamage = attack.GetValue() - targetStatus.defense.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        targetStatus.TakeDamage(totalDamage);
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // 当生命值归零 DIE
        if (CurrentHealth <= 0)
        {
            OnHealthReachedZero?.Invoke();
        }
    }

    /// <summary>
    /// 回复生命值
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth.GetValue());
    }
}