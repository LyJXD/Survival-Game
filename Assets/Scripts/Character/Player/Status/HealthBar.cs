using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private TextMeshProUGUI healthCounter;

    [SerializeField] private GameObject playerStatus;

    private int currentHealth;
    private int maxHealth;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        currentHealth = playerStatus.GetComponent<PlayerStatus>().CurrentHealth;
        maxHealth = playerStatus.GetComponent<PlayerStatus>().maxHealth.GetValue();

        float fillValue = currentHealth / maxHealth;
        slider.value = fillValue;

        healthCounter.text = currentHealth + "/" + maxHealth;
    }
}
