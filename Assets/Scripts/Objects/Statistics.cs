using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Statistics
{
    public float Health;
    public float MaxHealth;
    public float Mana;
    public float MaxMana;
    public float Damage;
    public float Speed;

    public Statistics()
    {
        Health = MaxHealth;
        Mana = MaxMana;
    }
}
