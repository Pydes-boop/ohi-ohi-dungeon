using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShadowKnightMovement : MonoBehaviour
{
    [SerializeField] private Vector2 movement;
    [SerializeField] private float maxTravelDistance;
    private Vector3 _movement;
    private Vector3 _startPoint;

    private void Awake()
    {
        _startPoint = transform.position;
        InitialiseValues();
    }

    private void OnEnable()
    {
        transform.position = _startPoint;
        AudioManager.Instance.AdjustLowPass(2000f);
        StartCoroutine(waitTime(4f));
    }

    void Update()
    {
        if (Vector3.SqrMagnitude(_startPoint - transform.position) >= maxTravelDistance * maxTravelDistance)
            gameObject.SetActive(false);
        transform.position += _movement * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 startPos = Application.isPlaying ? _startPoint : transform.position;
        Vector3 endPos = _startPoint + _movement.normalized * maxTravelDistance;
        Vector3 vertical = Vector3.Cross(_movement, Camera.main.transform.forward).normalized;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawLine(startPos + vertical, startPos - vertical);
        Gizmos.DrawLine(endPos + vertical, endPos - vertical);
    }

    private void OnValidate()
    {
        // Needed for OnDrawGizmos
        InitialiseValues();
    }

    private void InitialiseValues()
    {
        _movement = new Vector3(movement.x, movement.y, 0f);
        
        Vector3 scale = transform.localScale;
        scale.x = (movement.x < 0 || movement.y < 0 ? -1 : 1) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private IEnumerator waitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AudioManager.Instance.AdjustLowPass(22000);
    }
}
