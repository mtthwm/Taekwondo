using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public string[] playOnStart;
    public SoundGroup[] soundGroups;
    public BackgroundMusic[] songs;
    public string selectedBackgroundMusic;
    private BackgroundMusic b;

    void Awake () {
        foreach (Sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.clip = s.clip;
        }

        foreach (string s in playOnStart) 
        {
            Play(s);
        }

        foreach (SoundGroup g in soundGroups) 
        {
            foreach (Sound s in g.sounds) 
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.volume = s.volume;
                s.source.clip = s.clip;
            }
        }

        foreach (BackgroundMusic s in songs) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.clip = s.clip;
        }

        if (selectedBackgroundMusic != "") {
            b = Array.Find(songs, song => song.name == selectedBackgroundMusic);
            if (b != null) {
                b.source.Play();
            }
        }
    }

    public void Play (string name) {
        try {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.Play();
        } catch {
            Debug.Log("error");
        }
    }

    public void Stop (string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void StopBackgroundMusic () {
        b.source.Stop();
    }

    public void PlayRandomFromSoundGroup (string name) {
        SoundGroup group = Array.Find(soundGroups, g => g.name == name);
        Sound randSound = group.GetRandom();

        randSound.source.Play();
    }
}
