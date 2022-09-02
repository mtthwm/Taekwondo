using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchEndNotification : MonoBehaviour
{
    private Text text;
    private AudioManager audioManager;

    void Awake () {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void OnEnable () {
        text = gameObject.GetComponent<Text>();
        Timer.TimeStop += DisplayNotification;
        if (text != null) {
            text.enabled = false;
        }
    }

    void OnDisable () {
        Timer.TimeStop -= DisplayNotification;
    }

    void DisplayNotification () {
        if (audioManager != null) {
            audioManager.Play("Whistle");
            audioManager.StopBackgroundMusic();
        }
        if (text != null) {
            StartCoroutine(Blink(1.3f, 0.2f));
        }
    }

    IEnumerator Blink (float length, float blinkLen) {
        int iterations = (int) (length / blinkLen) * 2;
        for (int i = 0; i < iterations; i++) {
            text.enabled = !text.enabled;
            yield return new WaitForSeconds(blinkLen/2f);
        }
        text.enabled = true;
    }
}
