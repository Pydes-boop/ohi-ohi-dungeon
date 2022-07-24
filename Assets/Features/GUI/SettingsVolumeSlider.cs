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
    public void ChangeVolumeMusic(float newVolume) {
        AudioManager.Instance.AdjustMusicVolume(newVolume, 1);
    }

    public void ChangeVolumeSounds(float newVolume) {
        AudioManager.Instance.AdjustSoundsVolume(newVolume, 1);
    }
}
