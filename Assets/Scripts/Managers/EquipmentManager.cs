using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoSingleton<EquipmentManager>
{
    public List<InventorySlot> equipmentSlots;

    public Action<InventorySlot> OnEquipmentSlotChanged;
    public event Action<EquipmentItemData, EquipmentItemData> OnEquipmentChanged;

    public void UpdateEquipItemSlot(InventorySlot slot)
    {
        // ����updateSlot����
        OnEquipmentSlotChanged?.Invoke(slot);
    }

    public void ChangeEquipment(EquipmentItemData equipmentItemNew, EquipmentItemData equipmentItemOld)
    {
        OnEquipmentChanged?.Invoke(equipmentItemNew, equipmentItemOld);
    }
}
