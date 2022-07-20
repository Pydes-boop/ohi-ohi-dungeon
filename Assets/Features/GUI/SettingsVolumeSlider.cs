using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/*
 * Dumb script because i don't know how to reference a GameObject, that isn't in the scene yet
 */
public class SettingsVolumeSlider : MonoBehaviour
{
    private void Awake() {
        var content = GameObject.Find("Content").transform;
        for(int i = 0; i < content.childCount; i++) {
            content.GetChild(i).GetComponent<Slider>().value =
                AudioManager.instance.GetVolume(content.GetChild(i).name);
        }
    }

    public void ChangeVolumeMusic(float newVolume) {
        AudioManager.instance.ChangeVolume("GameOST", newVolume/2);
        AudioManager.instance.ChangeVolume("MenuOST", newVolume);
    }
    
    public void ChangeVolumeUI(float newVolume) => AudioManager.instance.ChangeVolume("ButtonPress", newVolume);
    public void ChangeVolumeEnemy(float newVolume) => AudioManager.instance.ChangeVolume("Coins", newVolume);
    public void ChangeVolumeCoin(float newVolume) => AudioManager.instance.ChangeVolume("Enemies", newVolume);
}
