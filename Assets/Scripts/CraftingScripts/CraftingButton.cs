using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
    public Button button;
    public CraftingRecipe recipe;

    private void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        CraftingManager.Instance.CraftItem(recipe);
    }
}
