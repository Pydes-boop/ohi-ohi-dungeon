using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PointerDragSensor : Sensor
{
    private void Awake()
    {
        SensorIsPressed = this.gameObject.AddComponent<ObservableEventTrigger>()
            .OnDragAsObservable();
    }
}
