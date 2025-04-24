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
        // ��������Ƿ��и���Ʒ
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

        // ��ȡ����е�һ���ղ�λ
        if(HasFreeSlot(out InventorySlot freeSlot))
        {
            // �����Ʒ����С�ڸ���Ʒ��λ�ѵ���������
            if (freeSlot.EnoughRoomLeftInStack(amountToAdd))
            {
                freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                OnInventorySlotChanged?.Invoke(freeSlot);
                return true;
            }

            // �����Ʒ�������ڸ���Ʒ��λ�ѵ���������
            // ...
        }

        return false;
    }

    // �ж�����Ƿ���ָ����Ʒ�����ش�Ÿ���Ʒ�Ĳ�λ
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

    // �ж����ʣ�����ܷ�װ�¹��ﳵ��������Ʒ
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

    // ��ȡ��ҿ����������Ʒ������
    public Dictionary<InventoryItemData, int> GetAllItemsInPlayerInventory()
    {
        // ����ҿ����Ʒ����
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

    // �ӿ�����Ƴ�ָ����Ʒ
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
