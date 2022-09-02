using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class SoundGroup
{
    public string name;
    public Sound[] sounds; 

    public Sound GetRandom () {
        System.Random random = new System.Random();

        Sound randomSound = sounds[random.Next(sounds.Length)];
        return randomSound;
    }
}
