using System;
using System.Collections;
using DyrdaDev.Singleton;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {
    
    public Sound[] sounds;

    public AudioMixer masterMixer;
    
    
    /// ------ Setup ------
    
    protected override void Awake()
    {
        base.Awake();
        
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clips[0];
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            AudioMixerGroup group = masterMixer.FindMatchingGroups(sound.mixerGroup)[0];
            if (group != null)
                sound.source.outputAudioMixerGroup = group;
            else
            {
                Debug.LogWarning(sound.name + " does not have a mixer group. It was added to the group Other");
                sound.source.outputAudioMixerGroup = masterMixer.FindMatchingGroups("Other")[0];
            }
            
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
    

    private void Start()
    {
        Play("MenuOST");
    }
    
    /// ------ Mixer Functions ------
    
    public void AdjustMasterVolume(float level, float maxLevel)
    {
        AdjustMixer("MasterVolume", level, maxLevel);
    }

    public void AdjustMusicVolume(float level, float maxLevel)
    {
        AdjustMixer("MusicVolume", level, maxLevel);
    }
    
    public void AdjustSoundsVolume(float level, float maxLevel)
    {
        AdjustMixer("SoundsVolume", level, maxLevel);
    }
    
    public void AdjustEnemiesVolume(float level, float maxLevel)
    {
        AdjustMixer("EnemiesVolume", level, maxLevel);
    }

    public void AdjustWorldVolume(float level, float maxLevel)
    {
        AdjustMixer("WorldVolume", level, maxLevel);
    }
    
    public void AdjustPlayerVolume(float level, float maxLevel)
    {
        AdjustMixer("PlayerVolume", level, maxLevel);
    }
    
    public void AdjustUIVolume(float level, float maxLevel)
    {
        AdjustMixer("UIVolume", level, maxLevel);
    }
    
    public void AdjustOtherVolume(float level, float maxLevel)
    {
        AdjustMixer("OtherVolume", level, maxLevel);
    }
    
    private void AdjustMixer(string name, float level, float maxLevel)
    {
        if (level <= 0)
        {
            masterMixer.SetFloat(name, -80);
        }
        else
        {
            //Default Operation for Calculating actual Decibel Values for Audio Levels
            masterMixer.SetFloat(name,  20 * (float) Math.Log10(level / maxLevel));
        }
    }


    /// ------ Audio Source Functions ------
    
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
        if(sound.clips != null)
            sound.source.PlayOneShot(sound.clips[UnityEngine.Random.Range(0, sound.clips.Length)]);
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
    
    /// ------ Step Sounds ------

    public void StartCharacterStepSounds()
    {
        StartCoroutine(stepsWhileWalkingOnScene());
    }
    
    private IEnumerator stepsWhileWalkingOnScene()
    {
        ChangePan("Steps", 1);
        for (int i = 0; i < 8; i++)
        {
            ChangePanRelative("Steps", -(1/(float)8));
            PlayOneShot("Steps");
            yield return new WaitForSeconds(.4f);
        }
    }
}
