using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem primaryInventorySystem;      // 所有拥有库存者都有的初级库存系统
    [SerializeField] protected int offset = 10;
    [SerializeField] protected int money;

    public InventorySystem PrimaryInventorySystem => primaryInventorySystem;
    public int Offset => offset;

    public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested;     // 展示库存系统，int代表库存中物品偏移量，即跳过前n个开始展示

    protected virtual void Awake()
    {
        SaveLoad.OnLoadGame += LoadInventory;

        primaryInventorySystem = new InventorySystem(inventorySize, money);    
    }

    protected abstract void LoadInventory(SaveData saveData);
}

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem InvSystem;
    public Vector3 Position;
    public Quaternion Rotation;

    public InventorySaveData(InventorySystem _invSystem, Vector3 _position, Quaternion _rotation)
    {
        InvSystem = _invSystem;
        Position = _position;
        Rotation = _rotation;
    }

    public InventorySaveData(InventorySystem _invSystem)
    {
        InvSystem = _invSystem;
        Position = Vector3.zero; 
        Rotation = Quaternion.identity;
    }    
}
