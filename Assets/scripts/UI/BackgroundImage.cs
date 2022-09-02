using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BackgroundImage : MonoBehaviour
{
    [SerializeField]
    private Background[] backgroundOptions;
    [SerializeField]
    private string overrideName;

    private Background currentBackground;
    private string backgroundName;
    private Image imageComponent;

    void OnEnable () {
        currentBackground = overrideName == "" ? backgroundOptions[0] : Array.Find(backgroundOptions, x => x.name == overrideName);
        imageComponent = gameObject.GetComponent<Image>();
        backgroundName = PlayerPrefs.GetString("level");
        if (overrideName == "") {
            currentBackground = Array.Find(backgroundOptions, x => x.name == backgroundName);
            if (currentBackground == null) {
                currentBackground = backgroundOptions[0];           
            }
        } else {
            currentBackground = Array.Find(backgroundOptions, x => x.name == overrideName);
            if (currentBackground == null) {
                currentBackground = backgroundOptions[0];           
            }
        }
        imageComponent.sprite = currentBackground.image;
    }
}
