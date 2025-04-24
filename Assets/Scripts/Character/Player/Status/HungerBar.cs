using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    private Slider Slider;
    [SerializeField] private TextMeshProUGUI healthCounter;

    [SerializeField] private GameObject playerStatus;

    private float currentHunger;
    private float maxHunger;

    private void Awake()
    {
        Slider = GetComponent<Slider>();
    }

    private void Update()
    {
        currentHunger = playerStatus.GetComponent<PlayerStatus>().currentHunger;
        maxHunger = playerStatus.GetComponent<PlayerStatus>().maxHunger.GetValue();

        float fillValue = currentHunger / maxHunger;
        Slider.value = fillValue;

        healthCounter.text = currentHunger + "/" + maxHunger;
    }
}
