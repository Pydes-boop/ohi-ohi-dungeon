using System;
using UnityEngine;

[Serializable]
public class Sound {

    public string name;

    public string mixerGroup;

    [Range(0f, 1f)]
    public float volume;
    
    [Range(.1f, 3f)]
    public float pitch;

    public bool loop;

    [Header("3D Settings")]
    public bool Spatial = false;

    [Range(-1f, 1f)]
    public float pan = 0;

    [HideInInspector]
    public AudioSource source;
    
    [SerializeField]
    public AudioClip[] clips;

}
