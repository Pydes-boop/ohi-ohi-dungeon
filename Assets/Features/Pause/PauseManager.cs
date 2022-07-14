using System;
using DyrdaDev.Singleton;
using UnityEngine;
using UniRx;

namespace Features.Pause
{
    public class PauseManager : SingletonMonoBehaviour<PauseManager>
    {
        // paused is needed for the inspector,
        // because the reactiveProperty does not send an event when changed in the editor
        [SerializeField] private bool paused = false;
        [HideInInspector] public BoolReactiveProperty isPaused = new BoolReactiveProperty(false);

        private float _oldTimeScale;

        private void OnEnable()
        {
            _oldTimeScale = 1f;
            isPaused.Subscribe(OnPauseAction).AddTo(this);
            isPaused.Subscribe(state => paused = state).AddTo(this);
        }

        private void OnValidate()
        {
            isPaused.Value = paused;
        }

        private void OnPauseAction(bool isNowPaused)
        {
            if (isNowPaused)
            {
                _oldTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = _oldTimeScale;
            }
        }
    }
}