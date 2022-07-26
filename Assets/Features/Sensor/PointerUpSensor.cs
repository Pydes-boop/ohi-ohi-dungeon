using System;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PointerUpSensor : Sensor
{
    private void Awake()
    {
        SensorTriggered = this.gameObject.AddComponent<ObservablePointerUpTrigger>()
            .OnPointerUpAsObservable()
            .Select(e => new SensorEventArgs(e));
    }
}
