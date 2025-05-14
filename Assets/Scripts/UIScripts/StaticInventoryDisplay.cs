using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static inventory display.
/// ��̬��ʾ�Ŀ��ϵͳ�����ײ������Ʒ��
/// </summary>
public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] 
    private InventoryHolder inventoryHolder;
    [SerializeField] 
    protected InventorySlotUI[] slots;

    protected virtual void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged += RefreshStaticDisplay;
    }

    protected virtual void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChanged -= RefreshStaticDisplay;
    }

    protected override void Start()
    {
        base.Start();

        RefreshStaticDisplay();
    }

    private void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.PrimaryInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else
        {
            Debug.LogWarning($"No inventory assigned to {this.gameObject}");
        }

        AssignSlot(inventorySystem, 0);
    }
    
    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();

        for (int i = 0; i < inventoryHolder.Offset; i++) 
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}
