using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot : ItemSlot
{
    public InventorySlot(InventoryItemData source, int amount)
    {
        itemData = source;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public InventorySlot()
    {
        ClearSlot();
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount)
    {
        itemData = data; 
        _itemID = itemData.ID;
        stackSize = amount;
    }

    // 检查槽位剩余空间是否足够
    public bool EnoughRoomLeftInStack(int amountToAdd, out int amountRemaining)   // out关键字在C#中用于方法参数，允许方法返回多个值。
    {
        amountRemaining = itemData.MaxStackSize - stackSize;

        return EnoughRoomLeftInStack(amountToAdd);
    }
    public bool EnoughRoomLeftInStack(int amountToAdd)
    {
        if (itemData == null || itemData != null && stackSize + amountToAdd <= itemData.MaxStackSize) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 对半分槽位中的物品
    public bool SplitStack(out InventorySlot splitStack)
    {
        if(stackSize <= 1)
        {
            splitStack = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt(stackSize / 2);
        RemoveFromStack(halfStack);
        splitStack = new InventorySlot(itemData, halfStack);

        return true;
    }
}
