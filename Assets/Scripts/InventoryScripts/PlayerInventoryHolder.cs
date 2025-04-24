using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInventoryHolder : InventoryHolder
{   
    public static UnityAction OnPlayerInventoryChanged;

    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    public bool isOpen;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        SaveGameManager.data.playerInventory = new InventorySaveData(primaryInventorySystem);
        Cursor.visible = false;

        isOpen = false;
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && !isOpen) 
        {
            // ��ҿ��չʾ���Եײ���Ʒ����Ŀ��Ϊƫ����
            OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, 7);

            isOpen = true;
        }       
        else if (Keyboard.current.eKey.wasPressedThisFrame && isOpen)
        {
            isOpen = false;
        }
    }

    protected override void LoadInventory(SaveData data)
    {
        // �������ӵı������ݣ����������ݣ������
        if (data.playerInventory.InvSystem != null)
        {
            this.primaryInventorySystem = data.playerInventory.InvSystem;
            OnPlayerInventoryChanged?.Invoke();
        }
    }

    public bool AddToInventory(InventoryItemData data, int amount)
    {
        if(primaryInventorySystem.AddToInventory(data, amount))
        {
            return true;
        }

        return false;
    }
}
