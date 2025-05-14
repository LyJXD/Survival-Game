using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField]
    private Image itemSprite;
    [SerializeField]
    private TextMeshProUGUI itemPrice;
    [SerializeField]
    private TextMeshProUGUI buyNumUI;
    [SerializeField]
    private ShopSlot assignedItemSlot;
    public ShopSlot AssignedItemSlot => assignedItemSlot;

    [SerializeField]
    private int buyNum;
    [SerializeField]
    private Button addButton;
    [SerializeField]
    private Button removeButton;

    public ShopKeeperDisplay ParentDisplay { get; private set; }
    public float MarkUp { get; private set; }

    private void Awake()
    {
        itemSprite.sprite = null;
        itemSprite.preserveAspect = true;
        itemSprite.color = Color.clear;
        itemPrice.text = "";
        buyNumUI.text = "0";
        buyNum = 0;

        addButton?.onClick.AddListener(AddItemToCart);
        removeButton?.onClick.AddListener(RemoveItemFromCart);
        ParentDisplay = transform.parent.GetComponentInParent<ShopKeeperDisplay>();
    }

    // 初始化商品栏位
    public void Init(ShopSlot slot, float markUp)
    {
        assignedItemSlot = slot;
        MarkUp = markUp;

        UpdateSlotUI();
    }

    // 更新商品栏UI
    private void UpdateSlotUI()
    {
        if(assignedItemSlot.ItemData!= null)
        {
            itemSprite.sprite = assignedItemSlot.ItemData.Icon;
            itemSprite.color = Color.white;
            var modifiedPrice = ShopKeeperDisplay.GetModifiedPrice(assignedItemSlot.ItemData, 1, MarkUp);
            var buyOrSell = ParentDisplay.isBuying ? "售价" : "收购价";
            itemPrice.text = $"{buyOrSell} - {modifiedPrice}";
            buyNumUI.text = $"{buyNum}";
        }
        else
        {
            itemSprite.sprite = null;
            itemSprite.color = Color.clear;
            itemPrice.text = "";
        }
    }

    private void AddItemToCart()
    {
        ParentDisplay.AddItemToCart(this);
        buyNum++;

        UpdateSlotUI();
    }

    private void RemoveItemFromCart()
    {
        ParentDisplay.RemoveItemFromCart(this);
        buyNum--;

        UpdateSlotUI();
    }
}
