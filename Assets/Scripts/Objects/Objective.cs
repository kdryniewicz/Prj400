using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ObjectiveItem", menuName = "Scriptable Objects/Objective Item", order = 1)]
[Serializable]
public class Objective : ScriptableObject
{
    public Sprite TaskGraphic; //Graphic that will be used in the HUD
    public String Name; //Representative name for the objective type.
    public GameObject SpawnableItem; //
    public Category ObjectiveType;
    public bool isToggled; // Will be used for the "toggle" objective.
    public int Score;

    // Start is called before the first frame update
    void Start()
    {
        TaskGraphic = SpawnableItem.GetComponent<Sprite>();
        if (ObjectiveType == Category.Find && Name.ToUpper() != "STAIRS")
        {
            if (SpawnableItem.GetComponent<Collectible>() != null)
                    SpawnableItem.GetComponent<Collectible>().score = Score;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
