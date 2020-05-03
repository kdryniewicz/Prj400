using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scr_LoadingBarScript : MonoBehaviour {
    //Timer to measure every how many "x" seconds/minutes etc to add to fill
    public CustomTimer t;
    //Sprite for the delay
    public Image spr;
    //Rate at which to keep adding to fill.
    public float ResetRate;
    //Bool to determine whether the fill is finished.
    public bool loadingFinished;
    //Text to display above action.
    public TextMeshProUGUI txtAction;


    // Use this for initialization
    void Start()
    {
        t.TimerID = "LoadingTimer";
        t.state = TimerState.Running;
        spr.fillAmount = 0;
        loadingFinished = false;

    }

    public void SetLoadingBarParams(string Text, float LoadRate)
    {
        txtAction.text = Text;
        ResetRate = LoadRate;
    }
    // Update is called once per frame
    void Update()
    {
        if (t.state == TimerState.Running)
        {
            t.time += Time.deltaTime;
        }
        else if (t.state == TimerState.Stopped)
        {
            t.time = -999f;
        }
        if (t.state == TimerState.Running & !loadingFinished)
        {
            if (t.time > ResetRate)
            {
                spr.fillAmount += 0.2f;
                t.time = 0;
            }
        }

        if (spr.fillAmount >= 1)
        {
            loadingFinished = true;
            t.state = TimerState.Stopped;
        }
        
    }
}
