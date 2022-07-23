using System;
using System.Collections;
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

            if (sound.Spatial) {
                sound.source.spatialBlend = 1;
                sound.source.panStereo = sound.pan;
            }

            if (sound.source.clip == null)
            {
                Debug.LogWarning("No Sound Clip found for " + sound.name);
            }
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

    public void PlayOneShot(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }
        if(sound.clip != null)
            sound.source.PlayOneShot(sound.clip);
    }

    public void ChangeVolume(string name, float newVolume) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }

        sound.source.volume = newVolume;
    }

    public float GetVolume(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return 0;
        }

        return sound.source.volume;
    }
    
    public void ChangePitch(string name, float newPitch) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }

        sound.source.pitch = newPitch;
    }
    
    public void ChangePan(string name, float newPan) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }

        sound.source.panStereo = newPan;
    }
    
    public void ChangePanRelative(string name, float adjustBy) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null) {
            Debug.LogWarning("Didn't find sound: " + name);
            return;
        }
        sound.source.panStereo += adjustBy;
    }

    public void StartCharacterStepSounds()
    {
        StartCoroutine(stepsWhileWalkingOnScene());
    }
    
    private IEnumerator stepsWhileWalkingOnScene()
    {
        AudioManager.instance.ChangePan("Steps", 1);
        for (int i = 0; i < 8; i++)
        {
            ChangePanRelative("Steps", -(1/(float)8));
            PlayOneShot("Steps");
            yield return new WaitForSeconds(.4f);
        }
    }
}
