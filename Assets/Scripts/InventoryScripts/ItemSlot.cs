using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSlot : ISerializationCallbackReceiver
{
    [NonSerialized] 
    protected InventoryItemData itemData;    // ��Ʒ��Ϣ
    [SerializeField] 
    protected int _itemID = -1;
    [SerializeField] 
    protected int stackSize;                 // ��Ʒ����

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;

    public void ClearSlot()
    {
        itemData = null;
        _itemID = -1;
        stackSize = -1;
    }

    // ������Ʒ����λ
    public void AssignItem(InventorySlot invSlot)
    {
        // ��λ������ͬ��Ʒ�����
        if (itemData == invSlot.itemData)
        {
            AddToStack(invSlot.stackSize);
        }
        // ��λ������ͬ��Ʒ���滻Ϊ����Ʒ
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
