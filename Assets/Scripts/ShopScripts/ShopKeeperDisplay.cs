using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopKeeperDisplay : MonoBehaviour
{
    [SerializeField]
    private ShopSlotUI shopSlotPrefab;

    [SerializeField]
    private GameObject _itemListContentPanel;

    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private TextMeshProUGUI buyButtonText;
    [SerializeField]
    private Button buyTab;
    [SerializeField]
    private Button sellTab;
    public bool isBuying;

    private ShopSystem shopSystem;
    private PlayerInventoryHolder playerInventoryHolder;

    /// <summary>
    /// 记录购物车商品 key代表商品信息 value代表商品数量
    /// </summary>
    private Dictionary<InventoryItemData, int> shoppingCart = new();

    // 记录购买价值总和
    private int buyTotal;
    [SerializeField]
    private TextMeshProUGUI buyTotalText;

    public void DisplayShopWindow(ShopSystem _shopSystem, PlayerInventoryHolder _playerInventoryHolder)
    {
        shopSystem = _shopSystem;
        playerInventoryHolder = _playerInventoryHolder;

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (buyButton != null)
        {
            buyButtonText.text = isBuying ? "购买" : "出售";
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(isBuying ? BuyItems : SellItems);
        }

        ClearSlots();

        buyTotal = 0;
        buyTotalText.enabled = false;
        buyButton.gameObject.SetActive(false);

        if (isBuying)
        {
            DisplayShopInventory();
        }
        else
        {
            DisplayPlayerInventory();
        }
    }

    /// <summary>
    /// 玩家从商店购买商品
    /// </summary>
    private void BuyItems()
    {
        if(playerInventoryHolder.PrimaryInventorySystem.Money < buyTotal)
        {
            UIManager.ShowPopInUI("金钱不足");
            return;
        }
        if(!playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(shoppingCart))
        {
            UIManager.ShowPopInUI("库存空间不足");
            return;
        }

        // 将购物车中商品加入玩家库存并扣除玩家金钱
        foreach (var kvp in shoppingCart)
        {
            shopSystem.PurchaseItem(kvp.Key, kvp.Value);

            for (int i = 0; i < kvp.Value; i++)
            {
                playerInventoryHolder.PrimaryInventorySystem.AddToInventory(kvp.Key, 1);
            }
        }
        playerInventoryHolder.PrimaryInventorySystem.SpendMoney(buyTotal);

        RefreshDisplay();
    }

    /// <summary>
    /// 玩家向商店出售商品
    /// </summary>
    private void SellItems()
    {
        foreach(var kvp in shoppingCart)
        {
            var price = GetModifiedPrice(kvp.Key, kvp.Value, shopSystem.SellMarkUp);

            shopSystem.SellItem(kvp.Key, kvp.Value);

            playerInventoryHolder.PrimaryInventorySystem.RemoveItemsFromInventory(kvp.Key, kvp.Value);
            playerInventoryHolder.PrimaryInventorySystem.EarnMoney(price);
        }

        RefreshDisplay();
    }

    private void ClearSlots()
    {
        shoppingCart = new Dictionary<InventoryItemData, int>();

        foreach (var item in _itemListContentPanel.transform.Cast<Transform>())
        {
            Destroy(item.gameObject);
        }
    }

    /// <summary>
    /// 玩家购买时展示商店库存商品
    /// </summary>
    private void DisplayShopInventory()
    {
        // 初始化商店商品
        foreach (var item in shopSystem.ShopInventory)
        {
            if(item.ItemData == null)
            {
                continue;
            }

            var shopSlot = Instantiate(shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(item, shopSystem.BuyMarkUp);
        }
    }

    /// <summary>
    /// 玩家出售时展示玩家库存物品
    /// </summary>
    private void DisplayPlayerInventory()
    {
        foreach (var item in playerInventoryHolder.PrimaryInventorySystem.GetAllItemsInPlayerInventory())
        {
            var tempSlot = new ShopSlot();
            tempSlot.AssignItem(item.Key, item.Value);

            var shopSlot = Instantiate(shopSlotPrefab, _itemListContentPanel.transform);
            shopSlot.Init(tempSlot, shopSystem.SellMarkUp);
        }
    }

    public void AddItemToCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);
        // 如果购物车中已有该商品
        if(shoppingCart.ContainsKey(data))
        {
            shoppingCart[data]++;          
        }
        // 如果购物车中还没有该商品
        else
        {
            shoppingCart.Add(data, 1);
        }
        buyTotal += price;
        buyTotalText.text = $"合计：{price}";

        if(buyTotal > 0 && !buyTotalText.IsActive())
        {
            buyTotalText.enabled = true;
            buyButton.gameObject.SetActive(true);
        }

        CheckCartVSAvailableMoney();
    }

    public void RemoveItemFromCart(ShopSlotUI shopSlotUI)
    {
        var data = shopSlotUI.AssignedItemSlot.ItemData;

        var price = GetModifiedPrice(data, 1, shopSlotUI.MarkUp);
        if (shoppingCart.ContainsKey(data))
        {
            shoppingCart[data]--;

            if (shoppingCart[data] < 0)
            {
                shoppingCart.Remove(data);
            }
        }
        buyTotal -= price;
        buyTotalText.text = $"合计：{price}";

        if (buyTotal <= 0 && buyTotalText.IsActive())
        {
            buyTotalText.enabled = false;
            buyButton.gameObject.SetActive(false);
            return;
        }

        CheckCartVSAvailableMoney();
    }

    /// <summary>
    /// 结合商店加价/减价幅度后的商品价格
    /// </summary>
    public static int GetModifiedPrice(InventoryItemData data, int amount, float markUp)
    {
        var baseValue = data.moneyValue * amount;

        return Mathf.FloorToInt(baseValue + baseValue * markUp);
    }

    /// <summary>
    /// 将购物车中商品总价与玩家所持货币进行比较
    /// </summary>
    private void CheckCartVSAvailableMoney()
    {
        buyTotalText.color = buyTotal > playerInventoryHolder.PrimaryInventorySystem.Money ? Color.red : Color.white;
    }

    public void OnBuyTabPressed()
    {
        isBuying = true;
        RefreshDisplay();
    }

    public void OnSellTabPressed()
    {
        isBuying = false;
        RefreshDisplay();
    }

}
