using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    private Button btn;
    [SerializeField]
    private string levelName;

    void OnEnable () {
        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(delegate {SetSelectedLevel(levelName);});
    }

    void SetSelectedLevel (string name) {
        PlayerPrefs.SetString("level", name);
        SceneManager.LoadScene("MultiplayerMatch");
    }
}
