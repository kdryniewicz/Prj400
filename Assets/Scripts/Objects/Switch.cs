using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public Sprite NormalState;
    public Sprite ToggledState;
    public bool isToggled;

    public void Start()
    {
        isToggled = false;
        GetComponent<SpriteRenderer>().sprite = NormalState;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isToggled)
        if (collision.CompareTag("Weapon"))
        {
            foreach (Task t in Task_Handler.tasks_instance.tasks)
            {
                if (t.goalToComplete.whatToAchieve.SpawnableItem.name + "(Clone)" == gameObject.name)
                {
                    t.goalToComplete.current++;
                    isToggled = true;
                    GetComponent<SpriteRenderer>().sprite = ToggledState;
                    break;
                }
            }
            //Destroy(gameObject);
        }
    }
}
