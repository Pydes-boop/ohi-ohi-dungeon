using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwingMorningStar : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;

    private Rigidbody2D _rigidbody;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();

        ObservableDragTrigger observable = hitBox.TryGetComponent(typeof(ObservableDragTrigger), out var component) ?
            (ObservableDragTrigger)component : hitBox.AddComponent<ObservableDragTrigger>();
        observable.OnDragAsObservable().Subscribe(OnDrag).AddTo(this);
    }

    private void OnDrag(PointerEventData data)
    {
        // Update Rotation of stick
        Vector2 dir = (Vector2)_camera.ScreenToWorldPoint(data.position) - _rigidbody.position;
        Quaternion quaternion = Quaternion.LookRotation(new Vector3(dir.x, dir.y, 0f), Vector3.back);
        _rigidbody.SetRotation(quaternion);
        Debug.DrawRay(_rigidbody.position, dir);
    }
}
