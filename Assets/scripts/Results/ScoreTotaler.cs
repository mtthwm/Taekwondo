using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ScoreTotaler : MonoBehaviour
{
    private int playerOneScore;
    private int playerTwoScore;
    private string winner;
    [SerializeField]
    private Text text;
    [SerializeField]
    private CameraShake cameraShake;
    [SerializeField]
    private Button btn;

    private AudioManager audioManager;

    void Awake () 
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void OnEnable()
    {
        playerOneScore = PlayerPrefs.GetInt("playerOneTempScore");
        playerTwoScore = PlayerPrefs.GetInt("playerTwoTempScore");

        if (playerOneScore < playerTwoScore) {
            winner = "Player One";
        } else {
            winner = "Player Two";
        }
        if (btn != null) {
            btn.onClick.AddListener(delegate {navigate("TitleScreen");});
            btn.gameObject.SetActive(false);
        }

        StartCoroutine(ShowWinner());
    }

    IEnumerator ShowWinner () {
        if (audioManager != null) {
            audioManager.Play("Clap");
        }
        yield return new WaitForSeconds(1.0f);

        if (text != null) {
            text.text = winner;
        }
        if (cameraShake != null) {
            cameraShake.shake(90, (float) Math.PI/4f, 1);
        }
        yield return new WaitForSeconds(2.0f);
        if (btn != null) {
            btn.gameObject.SetActive(true);
        }
    }

    void navigate (string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }    
}
