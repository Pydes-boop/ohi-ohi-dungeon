using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public void OnPause(bool paused)
    {
        PauseManager.Instance.isPaused.Value = paused;
    }
}
