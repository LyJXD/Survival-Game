using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// Base class for all stats: health, armor, damage etc 
/// </summary>
[System.Serializable]
public class Status
{
    public int baseValue;
    [SerializeField]
    private int currentValue;

    // Keep a list of all the modifiers on this stat
    private List<int> modifiers = new();

    // Add all modifiers together and return the result
    public int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        currentValue = finalValue;
        return finalValue;
    }

    // Add a new modifier to the list
    public void AddModifier(int modifier)
    {
        if (modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    // Remove a modifier from the list
    public void RemoveModifier(int modifier)
    {
        if (modifier != 0)
        {
            modifiers.Remove(modifier);
        }
    }
}