using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop System/Shop Item List" )]
public class ShopItemList : ScriptableObject
{
    public List<ShopInventoryItem> Items { get; private set; }
    // public int MaxAllowedMoney { get; private set; }
    // �չ����۷���
    public float SellMarkUp { get; private set; }
    // ���ۼӼ۷���
    public float BuyMarkUp { get; private set; }

}

[System.Serializable]
public struct ShopInventoryItem
{
    public InventoryItemData ItemData;
    public int Amount;
}