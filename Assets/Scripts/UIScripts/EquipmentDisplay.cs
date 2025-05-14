using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentDisplay : InventoryDisplay
{
    [SerializeField] 
    protected InventorySlotUI[] slots;

    protected override void Start()
    {
        base.Start();

        RefreshEquipmentDisplay();
    }

    private void RefreshEquipmentDisplay()
    {
        EquipmentManager.Instance.OnEquipmentSlotChanged += UpdateSlot;

        AssignSlot(inventorySystem, 0);
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlotUI, InventorySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            EquipmentManager.Instance.equipmentSlots.Add(new InventorySlot());
            slotDictionary.Add(slots[i], EquipmentManager.Instance.equipmentSlots[i]);
            slots[i].Init(EquipmentManager.Instance.equipmentSlots[i]);
        }
    }
}
