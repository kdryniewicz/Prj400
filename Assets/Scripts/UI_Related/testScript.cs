using ProceduralGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{

    public GameObject PlayerObj;
    private Statistics pStats;

    private Button btn;

    float dmg = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            PlayerObj = GameObject.FindWithTag("Player");
        }
        if(PlayerObj != null)
        {
            pStats = PlayerObj.GetComponent<Player>().PlayerStats;
        }
        btn = GetComponent<Button>();

        if (btn.gameObject.name.Contains("HP") && btn.gameObject.name.Contains("Add"))
        {
            btn.onClick.AddListener(AddHP);
        }
        else if (btn.gameObject.name.Contains("HP") && btn.gameObject.name.Contains("Remove"))
        {
            btn.onClick.AddListener(LoseHP);
        }
        else if (btn.gameObject.name.Contains("Mana") && btn.gameObject.name.Contains("Add"))
        {
            btn.onClick.AddListener(AddMana);
        }
        else if (btn.gameObject.name.Contains("Mana") && btn.gameObject.name.Contains("Remove"))
        {
            btn.onClick.AddListener(LoseMana);
        }
    }

    public void LoseHP()
    {
        pStats.Health -= dmg;
        Debug.Log(string.Format("{0} has lost {1} of health. He has {2} out of {3} left.", PlayerObj.name, dmg, PlayerObj.GetComponent<Player>().PlayerStats.Health, PlayerObj.GetComponent<Player>().PlayerStats.MaxHealth));
    }
    public void AddHP()
    {
        pStats.Health += dmg;
        Debug.Log(string.Format("{0} has gained {1} of health. He has {2} out of {3} left.", PlayerObj.name, dmg, PlayerObj.GetComponent<Player>().PlayerStats.Health, PlayerObj.GetComponent<Player>().PlayerStats.MaxHealth));
    }
    public void LoseMana()
    {
        pStats.Mana -= dmg;
        Debug.Log(string.Format("{0} has lost {1} of Mana. He has {2} out of {3} left.", PlayerObj.name, dmg, PlayerObj.GetComponent<Player>().PlayerStats.Mana, PlayerObj.GetComponent<Player>().PlayerStats.MaxMana));
    }
    public void AddMana()
    {
        pStats.Mana += dmg;
        Debug.Log(string.Format("{0} has gained {1} of Mana. He has {2} out of {3} left.", PlayerObj.name, dmg, PlayerObj.GetComponent<Player>().PlayerStats.Mana, PlayerObj.GetComponent<Player>().PlayerStats.MaxMana));
    }
    // Update is called once per frame
    void Update()
    {
        if(PlayerObj != GameObject.FindWithTag("Player"))
        {
            PlayerObj = GameObject.FindWithTag("Player");
        }
        if(pStats != PlayerObj.GetComponent<Player>().PlayerStats)
        {
            pStats = PlayerObj.GetComponent<Player>().PlayerStats;
        }
    }
}
