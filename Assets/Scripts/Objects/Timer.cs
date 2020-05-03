using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomTimer
{
    public string TimerID;
    public float time;
    public TimerState state;
}

public enum TimerState
{
    Stopped,
    Running,
    Started,
}