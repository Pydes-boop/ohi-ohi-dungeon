using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake() {

        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        transform.parent = null;    //So you can group it under "MANAGEMENT"
        DontDestroyOnLoad(gameObject);
        
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }
        
        sound.source.Play();
    }

    public void Pause(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }
        
        sound.source.Pause();
    }
    
    public void Stop(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }
        
        sound.source.Stop();
    }
}
