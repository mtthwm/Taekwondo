using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableImageOnTimerStop : MonoBehaviour
{
    void OnEnable () {
        Timer.TimeStop += HideImage;
    }

    void OnDisable () {
        Timer.TimeStop -= HideImage;
    }

    void HideImage () {
        Image image = gameObject.GetComponent<Image>();
        if (image) {
            image.enabled = false;
        }
    }
}

