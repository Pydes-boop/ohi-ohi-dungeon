using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TagTriggerEnter2DSensor : Sensor
{
    [SerializeField] private String compareTag;
    
    public void Awake()
    {
        SensorTriggered = this.gameObject.AddComponent<ObservableTrigger2DTrigger>()
            .OnTriggerEnter2DAsObservable()
            .Where(col => col.CompareTag(compareTag))
            .Select(e => EventArgs.Empty);
    }
}
