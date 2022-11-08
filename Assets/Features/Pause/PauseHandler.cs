using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseHandler : MonoBehaviour
{
    public GameObject settingsCanvas;

    private void Awake()
    {
        if (settingsCanvas == null)
        {
            Debug.LogError("Please set a Settings Canvas for your Pause Button in your normal Canvas");
        }
    }

    public void OnPause(bool paused)
    {
        PauseManager.Instance.isPaused.Value = paused;
        settingsCanvas.SetActive(paused);
    }
}
