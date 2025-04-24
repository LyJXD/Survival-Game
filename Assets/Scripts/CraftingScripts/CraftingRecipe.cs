using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting System/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public InventoryItemData itemData;

    public int RequirementsNum;
    public Requirement[] Requirements;
}

[System.Serializable]
public struct Requirement
{
    public InventoryItemData itemData;
    public int ReqNum;
}