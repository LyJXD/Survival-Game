using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : EntityStatus
{
    #region Status
    public Status agililty;             // 敏捷度
    public Status criticalRate;         // 暴击率
    public Status criticalDamage;       // 暴击伤害
    public Status maxHunger;            // 最大饱食度
    public Status attackToTree;           // 对树的攻击力 - 初始为0 有工具或者其他装备将增加该数值
    public Status attackToMineral;        // 对矿物的攻击力 - 初始为0 有工具或者其他装备将增加该数值
    #endregion

    public int currentHunger;
    private float distanceTravelled = 0f;
    private Vector3 lastPosition;

    public GameObject Player;

    public override void Start()
    {
        base.Start();

        currentHunger = maxHunger.GetValue();

        EquipmentManager.Instance.OnEquipmentChanged += ChangeEquipment;
    }

    private void Update()
    {
        HandleHunger();
    }

    public override void DoDamage(EntityStatus targetStatus)
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

    private void ChangeTool(ToolItemData newItem, ToolItemData oldItem)
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
    }

    private void ChangeEquipment(EquipmentItemData newItem, EquipmentItemData oldItem)
    {
        if (newItem != null)
        {
            maxHealth.AddModifier(newItem.health);
            attack.AddModifier(newItem.attack);
            defense.AddModifier(newItem.defense);
            agililty.AddModifier(newItem.agililty);
            criticalRate.AddModifier(newItem.criticalRate);
            criticalDamage.AddModifier(newItem.criticalDamage);
        }

        if (oldItem != null)
        {
            maxHealth.RemoveModifier(oldItem.health);
            attack.RemoveModifier(oldItem.attack);
            defense.RemoveModifier(oldItem.defense);
            agililty.RemoveModifier(oldItem.agililty);
            criticalRate.RemoveModifier(oldItem.criticalRate);
            criticalDamage.RemoveModifier(oldItem.criticalDamage);
        }
    }

    #region Health & Hunger
    private void HandleHunger()
    {
        distanceTravelled += Vector3.Distance(Player.transform.position, lastPosition);
        lastPosition = Player.transform.position;

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
