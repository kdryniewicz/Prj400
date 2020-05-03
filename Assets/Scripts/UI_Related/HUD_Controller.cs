using ProceduralGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Controller : MonoBehaviour
{
    //Statistic objects taken from Player that will be used to populate HUD
    public GameObject PlayerObj;
    public Statistics PlayerStats;

    public GameObject HealthObject;
    public GameObject ManaObject;

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.FindWithTag("Player") != null)
        {
            PlayerObj = GameObject.FindWithTag("Player");
        }
        else
        {
            Debug.Log("No Player script attached to player! Add it to player prefab or HUD will not function.");
        }
        PlayerStats = PlayerObj.GetComponent<Player>().PlayerStats;
        HealthObject.GetComponent<HUD_StatController>().SetupObject(PlayerStats.MaxHealth, PlayerStats.Health);
        ManaObject.GetComponent<HUD_StatController>().SetupObject(PlayerStats.MaxMana, PlayerStats.Mana);


    }

    private void LateUpdate()
    {
        if(PlayerStats != PlayerObj.GetComponent<Player>().PlayerStats)
        {
            PlayerStats = PlayerObj.GetComponent<Player>().PlayerStats;
        }
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if(PlayerStats.Health > PlayerStats.MaxHealth)
        {
            PlayerStats.Health = PlayerStats.MaxHealth;
        }
        if (PlayerStats.Mana > PlayerStats.MaxMana)
        {
            PlayerStats.Mana = PlayerStats.MaxMana;
        }
        HealthObject.GetComponent<HUD_StatController>().SetPlayerStat(PlayerStats.Health);
        ManaObject.GetComponent<HUD_StatController>().SetPlayerStat(PlayerStats.Mana);
    }
}
