using ProceduralGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int score = 0;
    public CollectibleType collType;
    public int healAmount = 0; //In Case of a potion
    //private void OnCollision2DEnter(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        if(collType == CollectibleType.Collectible)
    //        {
    //            GameManager.instance.TotalScore += score;
    //            foreach (Task t in Task_Handler.tasks_instance.tasks)
    //            {
    //                if (t.goalToComplete.whatToAchieve.SpawnableItem == this)
    //                {
    //                    t.goalToComplete.current++;
    //                    break;
    //                }
    //            }

    //        }
    //        else if(collType == CollectibleType.HealthPotion)
    //        {
    //            if(Player.playerInstance.PlayerStats.Health < Player.playerInstance.PlayerStats.MaxHealth + healAmount)
    //            {
    //                Player.playerInstance.PlayerStats.Health = Player.playerInstance.PlayerStats.MaxHealth;
    //            }
    //            else
    //            {
    //                Player.playerInstance.PlayerStats.Health += healAmount;
    //            }
    //        }
    //        else if(collType == CollectibleType.ManaPotion)
    //        {
    //            if (Player.playerInstance.PlayerStats.Mana < Player.playerInstance.PlayerStats.MaxMana + healAmount)
    //            {
    //                Player.playerInstance.PlayerStats.Mana = Player.playerInstance.PlayerStats.MaxMana;
    //            }
    //            else
    //            {
    //                Player.playerInstance.PlayerStats.Mana += healAmount;
    //            }
    //        }
    //    }
    //}

}

public enum CollectibleType
{
    Collectible,
    HealthPotion,
    ManaPotion,

}