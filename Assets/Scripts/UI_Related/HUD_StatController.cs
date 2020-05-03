using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_StatController : MonoBehaviour
{
    public GameObject Container;
    public GameObject UnitContainer;
    public GameObject UnitBar;

    public float distContainerUnits = 14f;
    public float UnitsPerStat = 2f;

    private float playerStat;
    private int amountCreated;

    public List<GameObject> BarContainers = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if (UnitContainer != null)
        {
            //A Unit Container has only "Bar" child object, so we assign that based on the container prefab.
            UnitBar = UnitContainer.transform.GetChild(0).gameObject;
        }
    }


    public void SetupObject(float maxStat, float currStat)
    {
        SetUpStatDisplay(maxStat);
        SetPlayerStat(currStat);
    }
    public void SetPlayerStat(float stat)
    {
        playerStat = stat;
        //Debug.Log(string.Format("player stat: {0} is {1}.", gameObject.name, stat));
        UpdateStatObjects();
    }


    void RedoStatObjects(float maxStat, float currStat)
    {
        BarContainers.Clear();
        amountCreated = 0;

        SetupObject(maxStat, currStat);

    }

    public void UpdateStatObjects()
    {
        //Check all children objects, and update last one's fill rate.
        if (BarContainers != null)
        {
            float rate = (playerStat / UnitsPerStat);
            for (int i = 0; i <= BarContainers.Count - 1; i++)
            {
                BarContainers[i].GetComponent<Image>().fillAmount = rate;
                rate -= BarContainers[i].GetComponent<Image>().fillAmount;
            }
           //sbug.Log("Rate is: " + rate);
        }
    }

    // Update is called once per frame
    void SetUpStatDisplay(float maxStat)
    {
        //Set up container bars based on values from PlayerStatistics and above settings.
        int bars = (int)(maxStat / UnitsPerStat);
        for (int i = 0; i < bars; i++)
        {
            float prevX = 0;
            if(i > 0)
            {
                prevX = Container.transform.GetChild(i - 1).position.x;
                Vector2 tempPos = new Vector2(prevX + distContainerUnits, 0.0f);
                GameObject go = Instantiate(UnitContainer, tempPos, Quaternion.Euler(0,0,0), Container.transform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, 0.0f, 0.0f);
                //We add the "stat" bar object into the list to look through later for updating, since each container only has one child, we know it's going to be at index 0.
                BarContainers.Add(go.transform.GetChild(0).gameObject);
            }
            else
            {
                GameObject go = Instantiate(UnitContainer, Container.transform);
                BarContainers.Add(go.transform.GetChild(0).gameObject);
            }
        }
        amountCreated = BarContainers.Count;
        //Debug.Log(string.Format("Amount of Containers created for {0} is {1}", gameObject.name, amountCreated));
    }
}
