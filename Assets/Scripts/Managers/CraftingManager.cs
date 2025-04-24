using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CraftingManager : MonoSingleton<CraftingManager>
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;
    public bool isOpen;

    public PlayerInventoryHolder inventory;

    // Category Buttons
    Button toolsBTN;
    Button exitBTN;

    private void Start()
    {
        isOpen = false;

        exitBTN = craftingScreenUI.transform.Find("ExitButton").GetComponent<Button>();
        exitBTN.onClick.AddListener(delegate { ExitCategory(); });

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });
    }

    private void OpenToolsCategory()
    {
        exitBTN.gameObject.SetActive(true);
        toolsBTN.gameObject.SetActive(false);
        toolsScreenUI.SetActive(true);
    }

    private void ExitCategory()
    {
        exitBTN.gameObject.SetActive(false);
        toolsBTN.gameObject.SetActive(true);
        toolsScreenUI.SetActive(false);
    }

    public void CraftItem(CraftingRecipe recipe)
    {
        InventoryItemData itemData = recipe.itemData;

        for (int i = 0; i < recipe.Requirements.Length; i++)
        {
            InventoryItemData reqItem = recipe.Requirements[i].itemData;
            int reqNum = recipe.Requirements[i].ReqNum;
            if (inventory.PrimaryInventorySystem.ContainsItem(reqItem, out List<InventorySlot> invSlot))
            {
                int tmpNum = reqNum;
                foreach (var slot in invSlot)
                {
                    if (slot.StackSize < tmpNum)
                    {
                        tmpNum -= slot.StackSize;
                    }
                }
                
                // 有足够的制作材料
                if (tmpNum <= 0)
                {
                    // 向库存添加制作的物品
                    if (inventory.AddToInventory(itemData, 1))
                    {
                        // 库存有空余位置添加物品
                        // 从库存中删除制作材料
                        inventory.PrimaryInventorySystem.RemoveItemsFromInventory(itemData, reqNum);
                    }
                    // 库存没有空余位置添加物品
                    else
                    {
                        UIManager.ShowPopInUI("库存空间不足");
                    }              
                }
                else
                {
                    return;
                }
            }
        }

          
    }

}