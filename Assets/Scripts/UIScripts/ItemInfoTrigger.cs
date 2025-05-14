using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInfoTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryItemData itemData;

    private CancellationTokenSource cts;

    private Vector2 pivot;

    private void Start()
    {
        cts = new CancellationTokenSource();
    }

    private void Update()
    {
        float pivotX = transform.position.x / Screen.width;
        float pivotY = transform.position.y / Screen.height;
        pivot = new Vector2 (pivotX, pivotY);

        if (!transform.GetComponent<CraftingButton>())
        {
            itemData = transform.GetComponent<InventorySlotUI>().AssignedInventorySlot.ItemData;
        }
        else
        {
            itemData = transform.GetComponent<CraftingButton>().recipe.itemData;
        }
    }

    // Triggered when the mouse enters into the area of the item that has this script.
    public async void OnPointerEnter(PointerEventData eventData)
    {
        bool cancel = await UniTask.Delay(TimeSpan.FromSeconds(0.3), cancellationToken: cts.Token).SuppressCancellationThrow();
        if (!cancel)
        {
            if(itemData != null)
            {
                UIManager.ShowItemInfoUI(itemData.DispalyName, itemData.Description, itemData.Function, transform.position, pivot, itemData);
            }
        }
    }

    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
        UIManager.HideItemInfoUI();
    }
}
