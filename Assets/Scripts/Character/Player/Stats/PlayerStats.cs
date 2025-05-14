using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{
    #region Stats
    public Stats maxHunger;            // 最大饱食度
    public Stats dropRate;             // 掉宝率
    public Stats criticalRate;         // 暴击率
    public Stats criticalDamage;       // 暴击伤害
    public Stats attackToTree;         // 对树的攻击力 - 初始为0 有工具或者其他装备将增加该数值
    public Stats attackToMineral;      // 对矿物的攻击力 - 初始为0 有工具或者其他装备将增加该数值
    #endregion

    public int currentHunger;
    private float distanceTravelled = 0f;
    private Vector3 lastPosition;

    public Player player;

    public event Action OnPlayerStatsChanged;

    public override void Start()
    {
        base.Start();

        player = PlayerManager.Instance.Player;

        currentHunger = maxHunger.GetValue();

        EquipmentManager.Instance.OnEquipmentChanged += ChangeEquipment;
        EquipmentManager.Instance.OnToolChanged += ChangeTool;
    }

    private void Update()
    {
        HandleHunger();
    }

    public override void DoDamage(EntityStats targetStatus)
    {
        int totalDamage = 0;

        if (SelectionManager.Instance.IsCharacterSelected)
        {
            totalDamage = attack.GetValue() - targetStatus.defense.GetValue();
        }
        if (SelectionManager.Instance.IsTreeSelected)
        {
            totalDamage = attackToTree.GetValue() - targetStatus.defense.GetValue();
        }
        if (SelectionManager.Instance.IsMineralSelected)
        {
            totalDamage = attackToMineral.GetValue() - targetStatus.defense.GetValue();
        }
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        targetStatus.TakeDamage(totalDamage);
    }

    private void ChangeTool(WeaponItemData newItem, WeaponItemData oldItem)
    {
        if (newItem != null)
        {
            attack.AddModifier(newItem.characterAttack);
            attackToTree.AddModifier(newItem.treeAttack);
            attackToMineral.AddModifier(newItem.mineralAttack);
        }

        if (oldItem != null)
        {
            attack.RemoveModifier(oldItem.characterAttack);
            attackToTree.RemoveModifier(oldItem.treeAttack);
            attackToMineral.RemoveModifier(oldItem.mineralAttack);
        }

        OnPlayerStatsChanged?.Invoke();
    }

    private void ChangeEquipment(EquipmentItemData newItem, EquipmentItemData oldItem)
    {
        if (newItem != null)
        {
            maxHealth.AddModifier(newItem.health);
            attack.AddModifier(newItem.attack);
            defense.AddModifier(newItem.defense);
            dropRate.AddModifier(newItem.dropRate);
            criticalRate.AddModifier(newItem.criticalRate);
            criticalDamage.AddModifier(newItem.criticalDamage);
        }

        if (oldItem != null)
        {
            maxHealth.RemoveModifier(oldItem.health);
            attack.RemoveModifier(oldItem.attack);
            defense.RemoveModifier(oldItem.defense);
            dropRate.RemoveModifier(oldItem.dropRate);
            criticalRate.RemoveModifier(oldItem.criticalRate);
            criticalDamage.RemoveModifier(oldItem.criticalDamage);
        }

        OnPlayerStatsChanged?.Invoke();
    }

    #region Health & Hunger
    private void HandleHunger()
    {
        distanceTravelled += Vector3.Distance(PlayerManager.Instance.Player.transform.position, lastPosition);
        lastPosition = PlayerManager.Instance.Player.transform.position;

        if (distanceTravelled >= 25)
        {
            distanceTravelled = 0;
            currentHunger -= 1;
        }
    }

    public void SetHealth(int health)
    {
        CurrentHealth = health;
    }

    public void SetHunger(int hunger)
    {
        currentHunger = hunger;
    }

    private void ConsumingFunction(int healthEffect, int hungerEffect)
    {
        HealthEffectCalculation(healthEffect);

        HungerEffectCalculation(hungerEffect);
    }

    private void HealthEffectCalculation(int healthEffect)
    {
        int healthBeforeConsumption = CurrentHealth;
        int maxHealthTmp = maxHealth.GetValue();

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealthTmp)
            {
                SetHealth(maxHealthTmp);
            }
            else
            {
                SetHealth(healthBeforeConsumption + healthEffect);
            }
        }
    }

    private void HungerEffectCalculation(int hungerEffect)
    {
        int hungerBeforeConsumption = currentHunger;
        int maxHungerTmp = maxHunger.GetValue();

        if (hungerEffect != 0)
        {
            if ((hungerBeforeConsumption + hungerEffect) > maxHungerTmp)
            {
                SetHunger(maxHungerTmp);
            }
            else
            {
                SetHunger(hungerBeforeConsumption + hungerEffect);
            }
        }
    }
    #endregion


}
