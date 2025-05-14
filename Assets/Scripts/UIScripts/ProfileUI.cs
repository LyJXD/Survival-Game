using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerHealth;
    [SerializeField]
    private TextMeshProUGUI playerHunger;
    [SerializeField]
    private TextMeshProUGUI playerAttack;
    [SerializeField]
    private TextMeshProUGUI playerDefense;
    [SerializeField]
    private TextMeshProUGUI playerDropRate;
    [SerializeField]
    private TextMeshProUGUI playerCriticalRate;
    [SerializeField]
    private TextMeshProUGUI playerCriticalDamage;

    private Player player;

    private void Start()
    {
        player = PlayerManager.Instance.Player;

        UpdatePlayerProfile();

        player.Stats.OnPlayerStatsChanged += UpdatePlayerProfile;      
    }

    private void UpdatePlayerProfile()
    {
        playerHealth.text = player.Stats.maxHealth.GetValue().ToString();
        playerHunger.text = player.Stats.maxHunger.GetValue().ToString();
        playerAttack.text = player.Stats.attack.GetValue().ToString();
        playerDefense.text = player.Stats.defense.GetValue().ToString();
        playerDropRate.text = player.Stats.dropRate.GetValue().ToString();
        playerCriticalRate.text = player.Stats.criticalRate.GetValue().ToString();
        playerCriticalDamage.text = player.Stats.criticalDamage.GetValue().ToString();
    }
}
