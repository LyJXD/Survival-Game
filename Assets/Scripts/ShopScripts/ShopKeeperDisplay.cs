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
    /// ��¼���ﳵ��Ʒ key������Ʒ��Ϣ value������Ʒ����
    /// </summary>
    private Dictionary<InventoryItemData, int> shoppingCart = new();

    // ��¼�����ֵ�ܺ�
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
            buyButtonText.text = isBuying ? "����" : "����";
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
    /// ��Ҵ��̵깺����Ʒ
    /// </summary>
    private void BuyItems()
    {
        if(playerInventoryHolder.PrimaryInventorySystem.Money < buyTotal)
        {
            UIManager.ShowPopInUI("��Ǯ����");
            return;
        }
        if(!playerInventoryHolder.PrimaryInventorySystem.CheckInventoryRemaining(shoppingCart))
        {
            UIManager.ShowPopInUI("���ռ䲻��");
            return;
        }

        // �����ﳵ����Ʒ������ҿ�沢�۳���ҽ�Ǯ
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
    /// ������̵������Ʒ
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
    /// ��ҹ���ʱչʾ�̵�����Ʒ
    /// </summary>
    private void DisplayShopInventory()
    {
        // ��ʼ���̵���Ʒ
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
    /// ��ҳ���ʱչʾ��ҿ����Ʒ
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
        // ������ﳵ�����и���Ʒ
        if(shoppingCart.ContainsKey(data))
        {
            shoppingCart[data]++;          
        }
        // ������ﳵ�л�û�и���Ʒ
        else
        {
            shoppingCart.Add(data, 1);
        }
        buyTotal += price;
        buyTotalText.text = $"�ϼƣ�{price}";

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
        buyTotalText.text = $"�ϼƣ�{price}";

        if (buyTotal <= 0 && buyTotalText.IsActive())
        {
            buyTotalText.enabled = false;
            buyButton.gameObject.SetActive(false);
            return;
        }

        CheckCartVSAvailableMoney();
    }

    /// <summary>
    /// ����̵�Ӽ�/���۷��Ⱥ����Ʒ�۸�
    /// </summary>
    public static int GetModifiedPrice(InventoryItemData data, int amount, float markUp)
    {
        var baseValue = data.moneyValue * amount;

        return Mathf.FloorToInt(baseValue + baseValue * markUp);
    }

    /// <summary>
    /// �����ﳵ����Ʒ�ܼ���������ֻ��ҽ��бȽ�
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
