﻿using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour
{
    public CoinAnimationController animationController;
    public Sensor collectSensor;
    public int value = 1;
    public AudioClip pickupSound;

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
        Collect();

        // Deactivate sensor.
        collectSensor.gameObject.SetActive(false);
    }

    public void Collect()
    {
        GameData.Instance.IncreaseScore(value);
        animationController.PlayCollectedAnimation();
        StartCoroutine(delaySFX());
        Destroy(gameObject, 3.0f);
    }

    IEnumerator delaySFX() {
        yield return new WaitForSeconds(.2f);
        AudioManager.instance.PlayOneShot("Coins", pickupSound);
    }
}