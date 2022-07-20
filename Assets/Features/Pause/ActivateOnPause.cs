using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActivateOnPause : MonoBehaviour
{
    [SerializeField] private bool invert;
    private void Start()
    {
        gameObject.SetActive(PauseManager.Instance.isPaused.Value);
        PauseManager.Instance.isPaused.Subscribe(paused => gameObject.SetActive(invert ? !paused : paused)).AddTo(this);
    }
}
