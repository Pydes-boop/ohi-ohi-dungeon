using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

public class MornigStarPhysicsController : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private GameObject stick;
    [SerializeField] private float strength;

    private Rigidbody2D _rigidbody;
    private Camera _camera;
    private float _targetDistance;
    
    private void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();

        // ObservableDragTrigger observable = hitBox.TryGetComponent(typeof(ObservableDragTrigger), out var component) ?
        //     (ObservableDragTrigger)component : hitBox.AddComponent<ObservableDragTrigger>();
        // observable.OnDragAsObservable().Subscribe(OnDrag).AddTo(this);

        _targetDistance = Vector2.Distance(transform.position, stick.transform.position);
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce((_rigidbody.position - (Vector2)stick.transform.position).normalized * strength * Time.fixedDeltaTime);
    }
}
