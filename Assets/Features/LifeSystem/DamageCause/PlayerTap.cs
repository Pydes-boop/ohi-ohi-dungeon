using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Sensor))]
public class PlayerTap : DamageCause// TODO remove comments and rename class
{
    // private Sensor _sensor;
    private Sensor[] _sensors;
    
    public DamageEffect damageEffect;
    
    [Header("VFX")]
    public GameObject damageVFX;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
        // _sensor = GetComponent<Sensor>();
        _sensors = GetComponents<Sensor>();
    }

    private void Start()
    {
        // _sensor.SensorTriggered.Subscribe(DamageCauseSignalDetected).AddTo(this);
        foreach (var sensor in _sensors)
        {
            if (typeof(PointerDragSensor) != sensor.GetType())
                sensor.SensorTriggered.Subscribe(DamageCauseSignalDetected).AddTo(this);
        }
    }

    public void DamageCauseSignalDetected(EventArgs args)
    {
        if (PauseManager.Instance.isPaused.Value)
            return;
        
        damageEffect.Trigger(this);
        
        if (args is SensorEventArgs && ((SensorEventArgs)args).associatedPointerPayload.position != null)
        {
            Vector3 pos = _camera.ScreenToWorldPoint(((SensorEventArgs)args).associatedPointerPayload.position);
            Instantiate(damageVFX, new Vector3(pos.x, pos.y, damageVFX.transform.position.z), Quaternion.identity);
            AudioManager.Instance.ChangePitch("Explosion", Random.Range(.8f, 1.2f));
            AudioManager.Instance.PlayOneShot("Explosion");
        }
    }
}
