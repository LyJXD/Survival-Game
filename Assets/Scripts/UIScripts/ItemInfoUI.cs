using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemFunction;
    public TextMeshProUGUI itemAdditionalInfo;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string name, string description, string function, InventoryItemData itemData)
    {
        itemName.text = $"【{name}】";
        itemDescription.text = $" {description}";
        itemFunction.text = $" {function}";
        itemAdditionalInfo.text = itemData.ShowAdditionalInfo();

        int nameLength = itemName.text.Length;
        int descriptionLength = itemDescription.text.Length;
        int funcLength = itemFunction.text.Length;
        int extraLength = GetEffectiveLength(itemAdditionalInfo.text);

        layoutElement.enabled = (nameLength > characterWrapLimit || descriptionLength > characterWrapLimit || funcLength > characterWrapLimit || extraLength > characterWrapLimit);
    }

    public void SetPosition(Vector2 position, Vector2 pivot)
    {
        transform.position = position;
        rectTransform.pivot = pivot;
    }

    private int GetEffectiveLength(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        if (text.Contains(Environment.NewLine))
        {
            return characterWrapLimit; // 把换行当作撑开一行处理
        }

        return text.Length;
    }

}
