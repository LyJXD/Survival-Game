using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShopSystem
{
    public List<ShopSlot> ShopInventory { get; private set; }       // 记录该商店售卖的商品库存
    // public int AvailableMoney { get; private set; }              // 记录商店持有的金钱
    public float BuyMarkUp { get; private set; }
    public float SellMarkUp { get; private set; }

    public ShopSystem(int _size, float _buyMarkUp, float _sellMarkUp)
    {
        //AvailableMoney = _money;
        BuyMarkUp = _buyMarkUp;
        SellMarkUp = _sellMarkUp;

        SetShopSize(_size);
    }

    private void SetShopSize(int size)
    {
        ShopInventory = new List<ShopSlot>(size);

        for (int i = 0; i < size; i++)
        {
            ShopInventory.Add(new ShopSlot());
        }
    }

    public void AddToShop(InventoryItemData data, int amount)
    {
        if(ContainsItem(data, out ShopSlot shopSlot))
        {
            shopSlot.AddToStack(amount);
            return;
        }

        var freeSlot = GetFreeSlot();

        freeSlot.AssignItem(data, amount);
    }

    private ShopSlot GetFreeSlot()
    {
        var freeSlot = ShopInventory.FirstOrDefault(i => i.ItemData == null);
        if (freeSlot == null)
        {
            freeSlot = new ShopSlot();
            ShopInventory.Add(freeSlot);
        }

        return freeSlot;
    }

    public bool ContainsItem(InventoryItemData itemToAdd, out ShopSlot shopSlot)
    {
        shopSlot = ShopInventory.Find(i => i.ItemData == itemToAdd);
        return shopSlot != null;
    }

    internal void PurchaseItem(InventoryItemData data, int amount)
    {
        if (!ContainsItem(data,out ShopSlot shopSlot))
        {
            return;
        }

        shopSlot.RemoveFromStack(amount);
    }

    internal void SellItem(InventoryItemData key, int value)
    {
        AddToShop(key, value);
    }
}
