using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System;

[System.Serializable]
public class InventorySystem
{
    [SerializeField]
    private List<InventorySlot> inventorySlots;

    private int _money;
    public int Money => _money;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => inventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size)
    {
        _money = 0;
        CreateInventory(size);
    }

    public InventorySystem(int size, int money)
    {
        _money = money;
        CreateInventory(size);
    }

    public void CreateInventory(int size)
    {
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        // 检查库存中是否有该物品
        if(ContainsItem(itemToAdd, out List<InventorySlot> invSlot))
        {
            foreach(var slot in invSlot)
            {
                if (slot.EnoughRoomLeftInStack(amountToAdd))
                {
                    slot.AddToStack(amountToAdd);
                    OnInventorySlotChanged?.Invoke(slot);
                    return true;
                }
            }
        }

        // 获取库存中第一个空槽位
        if(HasFreeSlot(out InventorySlot freeSlot))
        {
            // 添加物品数量小于该物品槽位堆叠数量上限
            if (freeSlot.EnoughRoomLeftInStack(amountToAdd))
            {
                freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                OnInventorySlotChanged?.Invoke(freeSlot);
                return true;
            }

            // 添加物品数量大于该物品槽位堆叠数量上限
            // ...
        }

        return false;
    }

    // 判断玩家是否有指定物品并返回存放该物品的槽位
    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();

        return invSlot == null ? false : true;
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);

        return freeSlot == null ? false : true;
    }

    // 判断玩家剩余库存能否装下购物车中所有商品
    public bool CheckInventoryRemaining(Dictionary<InventoryItemData, int> shoppingCart)
    {
        var clonedSystem = new InventorySystem(this.InventorySize);

        for (int i = 0; i < InventorySize; i++)
        {
            clonedSystem.InventorySlots[i].AssignItem(InventorySlots[i].ItemData, InventorySlots[i].StackSize);
        }

        foreach (var kvp in shoppingCart)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                if(!clonedSystem.AddToInventory(kvp.Key, 1))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    public void SpendMoney(int num)
    {
        _money -= num;
    }

    public void EarnMoney(int num)
    {
        _money += num;
    }

    // 获取玩家库存中所有物品并分类
    public Dictionary<InventoryItemData, int> GetAllItemsInPlayerInventory()
    {
        // 将玩家库存物品分类
        var distinctItems = new Dictionary<InventoryItemData, int>();

        foreach (var item in InventorySlots)
        {
            if(item.ItemData == null)
            {
                continue;
            }

            if (!distinctItems.ContainsKey(item.ItemData))
            {
                distinctItems.Add(item.ItemData, item.StackSize);
            }
            else
            {
                distinctItems[item.ItemData] += item.StackSize;
            }
        }

        return distinctItems;
    }

    // 从库存中移除指定物品
    internal void RemoveItemsFromInventory(InventoryItemData data, int amount)
    {
        if(ContainsItem(data, out List<InventorySlot> invSlot))
        {
            foreach (var slot in invSlot)
            {
                var stackSize = slot.StackSize;

                if(stackSize > amount)
                {
                    slot.RemoveFromStack(amount);
                }
                else
                {
                    slot.RemoveFromStack(stackSize);
                    amount -= stackSize;
                }

                OnInventorySlotChanged?.Invoke(slot);
            }
        }
    }
}
