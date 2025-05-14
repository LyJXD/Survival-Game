using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] 
    private MouseItemSlot mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> slotDictionary;   // ��Բ�λ��UI���λ������Ʒ����Ϣ
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<InventorySlotUI, InventorySlot> SlotDictionary => slotDictionary;

    protected virtual void Start() { }

    public abstract void AssignSlot(InventorySystem invToDisplay, int offset);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach(var slot in slotDictionary)
        {
            if(slot.Value == updatedSlot)               // InventorySlot_UI
            {
                slot.Key.UpdateUISlot(updatedSlot);     // InventorySlot
            }
        }
    }

    public void SlotClicked(InventorySlotUI clickedUISlot) 
    {
        bool isRightPressed = Mouse.current.rightButton.isPressed;
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        bool isEquipmentSlot = clickedUISlot.transform.parent.GetComponent<EquipmentDisplay>();
        bool isClickedSlotHasItem = clickedUISlot.AssignedInventorySlot.ItemData != null;
        bool isMouseSlotHasItem = mouseInventoryItem.AssignedInventorySlot.ItemData != null;

        // �������λΪװ����
        if (isEquipmentSlot)
        {
            SwapSlots(clickedUISlot); 
            return;
        }

        // �������λ����Ʒ�����δ������Ʒ��������ȡ�ò�λ��Ʒ
        if (isClickedSlotHasItem && !isMouseSlotHasItem) 
        {
            // ����Ҽ����ʱȡ��һ���λ����Ʒ
            if(isRightPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                clickedUISlot.UpdateUISlot();
                return;
            }
            else if (isShiftPressed)
            {

            }
            else
            {
                mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                return;
            }
        }

        // �������λû����Ʒ����������Ʒ���������Ʒ����ò�λ
        if (!isClickedSlotHasItem && isMouseSlotHasItem)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();
            mouseInventoryItem.ClearSlot();
            return;
        }

        // �������λ����Ʒ��ͬʱ��������Ʒ���������Ʒ����ò�λ
        if (isClickedSlotHasItem && isMouseSlotHasItem)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            
            // ������λ����Ʒ��ͬ
            if (isSameItem)
            {
                // �����������δ�����ѵ���������
                if (clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
                {
                    clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();
                    
                    mouseInventoryItem.ClearSlot();
                    return;
                }
                // ����������������ѵ���������
                else if (!clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
                {
                    // �ò�λ�Ѵ�ѵ��������ޣ�����
                    if(leftInStack < 1)
                    {
                        SwapSlots(clickedUISlot);
                        return;
                    }
                    // �ò�λδ��ѵ��������ޣ�����괦����
                    else
                    {
                        int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                        clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                        clickedUISlot.UpdateUISlot();

                        var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remainingOnMouse);
                        mouseInventoryItem.ClearSlot();
                        mouseInventoryItem.UpdateMouseSlot(newItem);
                        return;
                    }
                }                
            }          
            // ������λ����Ʒ����ͬ������
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }
    }

    private void SwapSlots(InventorySlotUI clickedUISlot)
    {
        // �������λΪװ����
        if (clickedUISlot.transform.parent.GetComponent<EquipmentDisplay>() != null)
        {
            if (mouseInventoryItem.AssignedInventorySlot.ItemData is EquipmentItemData)
            {
                EquipmentManager.Instance.UpdateEquipItemSlot(mouseInventoryItem.AssignedInventorySlot);
                EquipmentManager.Instance.ChangeEquipment((EquipmentItemData)mouseInventoryItem.AssignedInventorySlot.ItemData, (EquipmentItemData)clickedUISlot.AssignedInventorySlot.ItemData);
            }
            else
            {
                return;
            }
        }  
        
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();
        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();   
        
    }
}
