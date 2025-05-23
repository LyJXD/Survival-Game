using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ResourceHealthBar : MonoBehaviour
{
    private Slider slider;
    private float currentHealth, maxHealth;

    public GameObject globalState;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        currentHealth = globalState.GetComponent<EntityStats>().CurrentHealth;
        maxHealth = globalState.GetComponent<EntityStats>().maxHealth.GetValue();

        float fillValue = currentHealth / maxHealth;
        slider.value = fillValue;
    }
}
