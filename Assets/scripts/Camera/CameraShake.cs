using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class CameraShake : MonoBehaviour
{
    private float yPos;
    private float frequency;
    private float amplitude;
    private int duration;
    private int x;
    private float p;
    private int remainingFrame;

    void Start () {
        yPos = transform.position.y;
    }

    // Update is called once per frame
    void Update ()
    {
        // Debug.Log(remainingFrame);
        if (remainingFrame > 0) {
            x = duration - remainingFrame;
            float y =  (amplitude/x) * (float) Math.Sin(p * x);
            if (!(y != y)) {
                transform.position = new Vector3(transform.position.x, y + yPos, transform.position.z);
            }
            remainingFrame -= 1;
        } else {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
    }

    public void shake (int dur, float fre, float amp) 
    {
        p = dur / fre;
        frequency = fre;
        amplitude = amp;
        duration = dur;
        remainingFrame = dur;

    }

    // void OnValidate () 
    // {
    //     Debug.Log("VALIDATE");
    //     shake(30, (float) Math.PI/4f, 2);
    // }
}
