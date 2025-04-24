using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSlot : ISerializationCallbackReceiver
{
    [NonSerialized] 
    protected InventoryItemData itemData;    // 物品信息
    [SerializeField] 
    protected int _itemID = -1;
    [SerializeField] 
    protected int stackSize;                 // 物品数量

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;

    public void ClearSlot()
    {
        itemData = null;
        _itemID = -1;
        stackSize = -1;
    }

    // 分配物品给槽位
    public void AssignItem(InventorySlot invSlot)
    {
        // 槽位中有相同物品则添加
        if (itemData == invSlot.itemData)
        {
            AddToStack(invSlot.stackSize);
        }
        // 槽位中无相同物品则替换为该物品
        else
        {
            itemData = invSlot.itemData;
            _itemID = itemData.ID;
            stackSize = 0;
            AddToStack(invSlot.stackSize);
        }
    }

    public void AssignItem(InventoryItemData data, int amount)
    {
        if(itemData == data)
        {
            AddToStack(amount);
        }
        else
        {
            itemData = data;
            _itemID = data.ID;
            stackSize = 0;
            AddToStack(amount);
        }
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;

        if(stackSize <= 0)
        {
            ClearSlot();
        }
    }

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {
        if (_itemID == -1) return;

        var db = Resources.Load<Database>("Database");
        itemData = db.GetItem(_itemID);
    }
}
