using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UniqueID))]
public class ShopKeeper : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ShopItemList shopItemsHeld;
    [SerializeField]
    private ShopSystem shopSystem;

    private ShopSaveData _shopSaveData;
    private string _id;

    public UnityAction<IInteractable> OnInteractionComplete { get; set; }
    public static UnityAction<ShopSystem, PlayerInventoryHolder> OnShopWindowRequested { get; set; }

    private void Awake()
    {
        shopSystem = new ShopSystem(shopItemsHeld.Items.Count, shopItemsHeld.BuyMarkUp, shopItemsHeld.SellMarkUp);

        // 加载商店中的商品
        foreach (var item in shopItemsHeld.Items)
        {
            shopSystem.AddToShop(item.ItemData, item.Amount);
        }

        _id = GetComponent<UniqueID>().ID;
        _shopSaveData = new ShopSaveData(shopSystem);
    }

    private void Start()
    {
        if (!SaveGameManager.data.shopKeeperDictionary.ContainsKey(_id))
        {
            SaveGameManager.data.shopKeeperDictionary.Add(_id, _shopSaveData);
        }
    }

    private void OnEnable()
    {
        SaveLoad.OnLoadGame += LoadShopInventory;
    }

    private void OnDisable()
    {
        SaveLoad.OnLoadGame -= LoadShopInventory;
    }

    private void LoadShopInventory(SaveData data)
    {
        if(!data.shopKeeperDictionary.TryGetValue(_id, out ShopSaveData shopSaveData))
        {
            return;
        }

        _shopSaveData = shopSaveData;
        shopSystem = shopSaveData.ShopSystem;
    }

    public void Interact(Interactor interactor, out bool interactSuccess)
    {
        var playerInv = interactor.GetComponent<PlayerInventoryHolder>();

        if(playerInv != null) 
        { 
            OnShopWindowRequested?.Invoke(shopSystem, playerInv);
            interactSuccess = true;
        }
        else
        {
            interactSuccess= false;
        }
    }

    public void EndInteraction()
    {

    }

}

public class ShopSaveData
{
    public ShopSystem ShopSystem;

    public ShopSaveData(ShopSystem shopSystem)
    {
        ShopSystem = shopSystem;
    }
}
