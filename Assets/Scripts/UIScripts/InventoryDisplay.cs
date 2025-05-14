using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] 
    private MouseItemSlot mouseInventoryItem;
    protected InventorySystem inventorySystem;
    protected Dictionary<InventorySlotUI, InventorySlot> slotDictionary;   // 配对槽位的UI与槽位持有物品的信息
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

        // 被点击槽位为装备槽
        if (isEquipmentSlot)
        {
            SwapSlots(clickedUISlot); 
            return;
        }

        // 被点击槽位有物品，鼠标未持有物品，则鼠标获取该槽位物品
        if (isClickedSlotHasItem && !isMouseSlotHasItem) 
        {
            // 鼠标右键点击时取走一半槽位中物品
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

        // 被点击槽位没有物品，鼠标持有物品，则鼠标物品填入该槽位
        if (!isClickedSlotHasItem && isMouseSlotHasItem)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();
            mouseInventoryItem.ClearSlot();
            return;
        }

        // 被点击槽位有物品，同时鼠标持有物品，则鼠标物品填入该槽位
        if (isClickedSlotHasItem && isMouseSlotHasItem)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            
            // 鼠标与槽位中物品相同
            if (isSameItem)
            {
                // 两者相加数量未超过堆叠数量上限
                if (clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
                {
                    clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();
                    
                    mouseInventoryItem.ClearSlot();
                    return;
                }
                // 两者相加数量超过堆叠数量上限
                else if (!clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
                {
                    // 该槽位已达堆叠数量上限，交换
                    if(leftInStack < 1)
                    {
                        SwapSlots(clickedUISlot);
                        return;
                    }
                    // 该槽位未达堆叠数量上限，从鼠标处补足
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
            // 鼠标与槽位中物品不相同，交换
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }
    }

    private void SwapSlots(InventorySlotUI clickedUISlot)
    {
        // 被点击槽位为装备槽
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
