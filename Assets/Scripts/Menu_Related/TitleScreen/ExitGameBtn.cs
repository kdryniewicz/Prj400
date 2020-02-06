using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameBtn : MonoBehaviour
{
    private Button btnExit;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<Button>() != null)
        {
            btnExit = gameObject.GetComponent<Button>();
        }
        if (btnExit != null)
        {
            btnExit.onClick.AddListener(ExitGame);
        }
    }

    // Update is called once per frame
    void ExitGame()
    {
        Application.Quit();   
    }
}
