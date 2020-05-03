using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasManager : MonoBehaviour
{
    public static UICanvasManager canvasInstance = null;				//Static instance of GameManager which allows it to be accessed by any other script.


    // Start is called before the first frame update
    void Start()
    {
        //Check if instance already exists
        if (canvasInstance == null)

            //if not, set instance to this
            canvasInstance = this;

        //If instance already exists and it's not this:
        else if (canvasInstance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

}
