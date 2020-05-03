using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Task
{
    public string Name;
    public string Description;
    public bool isActive;
    public bool isComplete;
    public bool isLast;
    public Goal goalToComplete;


    public Task()
    {

    }
    public Task(string name, string desc)
    {
        Name = name;
        Description = desc;
        if (Name.ToUpper().Contains("EXIT"))
        {
            isLast = true;
        }
        else
        {
            isLast = false;
        }

        if (isLast)
        {
            isActive = true;
        }
    }


    public override string ToString()
    {
        return string.Format("- {0}\n: Complete = {1}", Name, isComplete);
    }

   
}
[Serializable]
public class Goal
{
    public Category goalToDo;
    public IntRange amounts;
    public Objective whatToAchieve;
    public int MaxAmount;
    public int current;


    public override string ToString()
    {
        return string.Format("{0} of {1} {2}(s) to {3}", current, amounts.m_Max, whatToAchieve.Name, Enum.GetName(typeof(Category), goalToDo));
    }
}

[Serializable]
public enum Category
{
    Find,
    Kill,
    Toggle
}