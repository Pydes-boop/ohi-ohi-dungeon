using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

public class MorningStarPhysicsController : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject stick;
    [SerializeField] private float force;
    [SerializeField] private float groundingVelocity;
    [SerializeField] private float groundDrag;

    private Rigidbody2D _rigidbody;
    private float _oldDrag;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _oldDrag = _rigidbody.drag;

        ObservableDragTrigger observable = hitBox.TryGetComponent(typeof(ObservableDragTrigger), out var component) ?
            (ObservableDragTrigger)component : hitBox.AddComponent<ObservableDragTrigger>();
        observable.OnDragAsObservable().Subscribe(OnDrag).AddTo(this);
    }

    private void Start()
    {
        hitBox.GetComponent<PointerDownSensor>().SensorTriggered.Subscribe(_ =>
            {
                enabled = false;
                _rigidbody.drag = _oldDrag;
            })
            .AddTo(this);
        hitBox.AddComponent<ObservablePointerUpTrigger>().OnPointerUpAsObservable().Subscribe(_ => enabled = true)
            .AddTo(this);
    }

    private void FixedUpdate()
    {
        if (_rigidbody.velocity.sqrMagnitude < groundingVelocity * groundingVelocity)
        {
            _rigidbody.drag = groundDrag;
            // TODO play sound
            Debug.Log("OnGround");
        }
    }

    private void OnDrag(PointerEventData data)
    {
        _rigidbody.AddForce((_rigidbody.position - (Vector2)stick.transform.position).normalized * force * Time.deltaTime);
    }
}
