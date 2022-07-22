using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class RotateTest : MonoBehaviour
{
    [SerializeField] private float rotspeed;
    [SerializeField] private bool dir;

    private Rigidbody2D _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _rb.MoveRotation(_rb.rotation + rotspeed * Time.deltaTime * (dir ? -1 : 1));
    }
}
