using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenButtons : MonoBehaviour
{
    public Button start, exit; 

    // Start is called before the first frame update
    void Start()
    {
        if (start != null) {
            start.onClick.AddListener(delegate {navigate("LevelSelect");});
        }
        if (exit != null) {
            exit.onClick.AddListener(quit);
        }
    }

    void navigate (string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    void quit ()
    {
        Application.Quit();
    }
}
