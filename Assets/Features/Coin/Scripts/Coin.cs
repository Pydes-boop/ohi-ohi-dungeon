using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour
{
    public CoinAnimationController animationController;
    public Sensor collectSensor;
    public int value = 1;

    private void Start()
    {
        collectSensor.SensorTriggered.Subscribe(CollectSignalDetected).AddTo(this);
    }

    private void Awake()
    {
        animationController.PlaySpawnAnimation();
    }

    public void CollectSignalDetected(EventArgs args)
    {
        if (PauseManager.Instance.isPaused.Value)
            return;
        
        Collect();

        // Deactivate sensor.
        collectSensor.gameObject.SetActive(false);
    }

    public void Collect()
    {
        GameData.Instance.IncreaseScore(value);
        animationController.PlayCollectedAnimation();
        AudioManager.Instance.PlayOneShot("Coins");
        Destroy(gameObject, 3.0f);
    }
}