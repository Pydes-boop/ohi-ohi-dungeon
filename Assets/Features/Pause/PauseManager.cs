using System;
using DyrdaDev.Singleton;
using UnityEngine;
using UniRx;


public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
    // paused is needed for the inspector,
    // because the reactiveProperty does not send an event when changed in the editor
    [SerializeField] private bool paused = false;
    [HideInInspector] public BoolReactiveProperty isPaused = new BoolReactiveProperty(false);

    //used to temp save enemy speeds, dont know if this is necessary because idk if all enemies have the same speed or not
    private float[] enemySpeed;

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
        /*if (isNowPaused)
        {
            EnemyMovement[] allEnemies = Array.ConvertAll(GameObject.FindGameObjectsWithTag("Enemy"), (go) => go.GetComponent<EnemyMovement>());
            enemySpeed = new float[allEnemies.Length];
            for (int i = 0; i < allEnemies.Length; i++)
            {
                enemySpeed[i] = allEnemies[i].speed;
                allEnemies[i].speed = 0;
            } 
        }
        else
        {
            EnemyMovement[] allEnemies = Array.ConvertAll(GameObject.FindGameObjectsWithTag("Enemy"), (go) => go.GetComponent<EnemyMovement>());
            for (int i = 0; i < allEnemies.Length; i++)
            {
                allEnemies[i].speed = enemySpeed[i];
            } 
        }*/
        
        
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