using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameBtn : MonoBehaviour
{
    private Button btnStart;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.GetComponent<Button>() != null)
        {
            btnStart = gameObject.GetComponent<Button>();
        }
        if(btnStart != null)
        {
            btnStart.onClick.AddListener(StartGame);
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene("GameWorld", LoadSceneMode.Single);

    }
}
