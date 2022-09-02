using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    //Event Vars
    public delegate void timeStartEvent();
    public static event timeStartEvent TimeStart;
    public delegate void timeStopEvent();
    public static event timeStopEvent TimeStop;

    [SerializeField]
    private Text textComponent;
    [SerializeField]
    private int secondsAmount;
    [HideInInspector]
    public int SecondsAmount {
        get {
            return secondsAmount;
        }
    }

    private int startTime;
    private bool timerRunning = false;
    private int timeElapsed;

    void Start () {
        StartTimer();
    }


    void FixedUpdate()
    {
        if (timerRunning) {
            timeElapsed = (int)Time.time - startTime;
            if (textComponent != null) {
                TimeSpan t = TimeSpan.FromSeconds((double)(secondsAmount - timeElapsed));
                textComponent.text = t.ToString();
            }
            if (timeElapsed >= secondsAmount) {
                timerRunning = false;
                TimeStop();
            }
        }
    }

    public void StartTimer () {
        TimeStart();
        timerRunning = true;
        startTime = (int)Time.time;
    }

    public void StopTimer () {
        TimeStop();
        timerRunning = false;
    }

    void OnValidate () {
        if (textComponent != null) {
            TimeSpan t = TimeSpan.FromSeconds((double)(secondsAmount));
            textComponent.text = t.ToString();
        }
    }

}
