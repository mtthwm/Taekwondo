using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableTextOnTimerStop : MonoBehaviour
{
    void OnEnable () {
        Timer.TimeStop += HideText;
    }

    void OnDisable () {
        Timer.TimeStop -= HideText;
    }

    void HideText () {
        Text text = gameObject.GetComponent<Text>();
        if (text) {
            text.enabled = false;
        }
    }
}

