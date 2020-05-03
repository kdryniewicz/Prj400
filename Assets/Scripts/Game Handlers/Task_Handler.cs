using ProceduralGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task_Handler : MonoBehaviour
{
    public static Task_Handler tasks_instance;

    public List<Task> tasks = new List<Task>();             //List of possible tasks to generate during playthrough
    public IntRange taskRange = new IntRange(1, 2);          //Range of amounts of objectives to create during playthrough. There always will have to be at default one (Get to exit).
    public int TaskAmount;
    public List<GameObject> TaskGOs = new List<GameObject>(); //Keeps track of all tasks game objects that will be instantiated in game.
    public GameObject TaskUI;                               //Task UI text element to use as template for task generation.
    public Transform taskSpawnPoint;                        //Spawn point to instantiate the tasks game objects into.
    public float distBetweenTasks;
    public List<Objective> possibleObjectives;
    

    public void GenerateTasks()
    {
        taskSpawnPoint = GameObject.FindWithTag("tasksDisplay").transform;
        Task temp;
        TaskAmount = taskRange.Random;
        if (TaskAmount > 1)
        {
            for (int i = 0; i < TaskAmount; i++)
            {
                //Generate tasks here
                Category random = (Category)UnityEngine.Random.Range(0, 2);              
                Goal goal = new Goal() { amounts = new IntRange(1,3)};
                goal.MaxAmount = goal.amounts.Random;
                goal = GenGoal(1, goal.MaxAmount, random, possibleObjectives);

                if (i == TaskAmount - 1)
                {
                    temp = new Task() { goalToComplete = createDefaultGoal(possibleObjectives), isActive = true, isLast = true };
                    Debug.Log("Generated default exit task");
                    tasks.Add(temp);
                }
                else
                {

                    //if it's not the last task, generate one.
                    temp = new Task() { goalToComplete = goal, isActive = true, isComplete = false };
                    tasks.Add(temp);
                }
                //Debug.Log(string.Format("Generated a task of: {0}, category: {1} ", goal.ToString(), Enum.GetName(typeof(Category), random)));

                temp.goalToComplete.whatToAchieve.TaskGraphic = temp.goalToComplete.whatToAchieve.SpawnableItem.GetComponent<SpriteRenderer>().sprite;
                temp.Name = temp.goalToComplete.ToString();
            }
        }
        else
        {
            //In case of one task, generate only default one
            temp = new Task() { goalToComplete = createDefaultGoal(possibleObjectives), isActive = true, isLast = true };
            Debug.Log("Generated default exit task");
            tasks.Add(temp);
        }
     

        //Create the HUD element of Tasks to display here
        for (int i = 0; i < tasks.Count; i++)
        {
            float prevY = 0;
            if (i > 0)
            {
                prevY = taskSpawnPoint.transform.GetChild(i - 1).position.y;
                Vector2 tempPos = new Vector2(0.0f, prevY - distBetweenTasks);
                GameObject go = Instantiate(TaskUI, tempPos, Quaternion.Euler(0, 0, 0), taskSpawnPoint.transform);

                go.transform.localPosition = new Vector3(0.0f, go.transform.localPosition.y, 0.0f);
                go.GetComponentInChildren<TextMeshProUGUI>().text = tasks[i].goalToComplete.ToString();
                go.GetComponentInChildren<Image>().sprite = tasks[i].goalToComplete.whatToAchieve.TaskGraphic;
                TaskGOs.Add(go);
            }
            else
            {
                GameObject go = Instantiate(TaskUI, taskSpawnPoint);
                go.GetComponentInChildren<TextMeshProUGUI>().text = tasks[i].ToString();
                go.GetComponentInChildren<Image>().sprite = tasks[i].goalToComplete.whatToAchieve.TaskGraphic;

                TaskGOs.Add(go);
            }
        }

        List<GameObject> objectiveItems = new List<GameObject>();

        //Add all the generated tasks' items to generatin to make sure there's definitely enough to complete tasks.
        foreach (Task t in tasks)
        {
            for (int i = 0; i < t.goalToComplete.MaxAmount; i++)
            {
                if (t.goalToComplete.whatToAchieve.SpawnableItem.tag.ToUpper() != "EXIT")
                    objectiveItems.Add(t.goalToComplete.whatToAchieve.SpawnableItem);
            }
        }
    }

    void updateTaskObjects()
    {
        for (int i = 0; i < TaskGOs.Count; i++)
        {
            if (tasks[i].goalToComplete.current == tasks[i].goalToComplete.MaxAmount)
            {
                tasks[i].isComplete = true;
            }
            TaskGOs[i].GetComponentInChildren<TextMeshProUGUI>().text = tasks[i].goalToComplete.ToString();
        }
    }

    //Used to find the default goal of just getting to the exit.
    public Goal createDefaultGoal(List<Objective> objList)
    {
        foreach (Objective obj in objList)
        {
            if(obj.SpawnableItem.tag.ToUpper() == "EXIT")
            {
                return new Goal() { goalToDo = obj.ObjectiveType, amounts = new IntRange(1,1), whatToAchieve = obj, MaxAmount = 1, current = 1};
            }
        }
        return new Goal();
    }


    public Goal GenGoal(int minVal, int maxVal, Category cat, List<Objective> objList)
    {
        List<Objective> objectives = new List<Objective>(); // First we create an empty list to store the objectives picked out with the assigned category0

        //We do a loop to pick the objectives out and add them to the list above
        foreach (Objective ob in objList)
        { //Check to make sure the category matches and not to pick the stairs (since we want it always as the last one)
            if (ob.ObjectiveType == cat && ob.SpawnableItem.tag.ToUpper() != "EXIT" && cat != Category.Kill)
            {
                objectives.Add(ob);
            }
            else if (ob.ObjectiveType == cat && cat == Category.Kill)
            {
                foreach (GameObject enemy in GameManager.instance.GetComponent<BoardCreator>().enemiesCreated)
                {
                    if (enemy.GetComponent<Enemy>().enemyName.ToUpper().Contains(ob.SpawnableItem.name.ToUpper()))
                    {
                        ob.TaskGraphic = enemy.GetComponent<SpriteRenderer>().sprite;
                        ob.SpawnableItem = enemy;
                        objectives.Add(ob);
                        break;
                    }
                }
            }
        }

        //now we'll need to pick one randomly from the range of objectives matched previously.
        IntRange indRange = new IntRange(0, objectives.Count);
        //Debug.Log(string.Format("Objectives count {0:} + indRange random: {1}", objectives.Count, indRange.Random));
        int idx = indRange.Random;
        //Debug.Log(string.Format("Amount of possible objectives: {0} and idx: {1}", objectives.Count, objectives[idx].ToString()));
        if(idx < 0)
        {
            idx = 0;
        }

        //Then we construct and return a generated "Goal" with what we came up before.
        return new Goal() { goalToDo = cat, amounts = new IntRange(minVal, maxVal), whatToAchieve = objectives[idx], MaxAmount = maxVal, current = 0 };

    }
    

    void updateTasks()
    {
        int tasksCompleted = 0;

        foreach (Task t in tasks)
        {
            if(t.goalToComplete.current == t.goalToComplete.MaxAmount && !(t.goalToComplete.whatToAchieve.SpawnableItem.tag.ToUpper() == "EXIT"))
            {
                t.isComplete = true;
                tasksCompleted++;
            }
            if (tasksCompleted == TaskAmount - 1)
            {
                if (t.isLast && t.goalToComplete.whatToAchieve.SpawnableItem.tag.ToUpper() == "EXIT")
                {
                    t.isActive = true;
                    GameManager.instance.Stairs.SetActive(true);
                }
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        GenerateTasks();
        //Check if instance already exists
        if (tasks_instance == null)

            //if not, set instance to this
            tasks_instance = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        updateTasks();

        updateTaskObjects();
    }
}
