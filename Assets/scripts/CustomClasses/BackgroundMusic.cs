using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundMusic
{
    public string name;
    public AudioClip clip;
    [HideInInspector] public AudioSource source;
    [Range(0.0f, 1.0f)] public float volume;
}
