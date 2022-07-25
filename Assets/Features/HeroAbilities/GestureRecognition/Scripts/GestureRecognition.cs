using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.Assertions.Comparers;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Sensor))]
public class GestureRecognition : MonoBehaviour
{
    public GameObject triangleAbility;
    public GameObject squareAbility;
    
    private PointerDragSensor _dragSensor;
    private PointerDownSensor _downSensor;
    private BoxCollider2D _collider;

    public GameObject particleSystem;
    public GameObject enviroment;

    [SerializeField] private float allowedOffsetX = 0.1f;
    [SerializeField] private float allowedOffsetY = 0.1f;

    private List<Vector2> _drawPoints = new List<Vector2>();
    private bool _actionStarted = false;
    private int _point = 0;
    private List<bool> _pointsReached = new List<bool>();

    private bool _eventStarted;
    private float _eventTimer;

    public float timeUntilNextGesture;
    private float _cooldownTimer;

    private int _switchGesture;

    private List<GameObject> _abilityPoints = new List<GameObject>();

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.enabled = true;
        _dragSensor = GetComponent<PointerDragSensor>();
        _downSensor = GetComponent<PointerDownSensor>();
        
        if(_collider == null)
            Debug.LogError("Gesture Recognition needs a Collider to function!");
        
        if(_dragSensor == null)
            Debug.LogError("Please add PointerDragSensor to Gesture Recognizer!");
        
        if(_downSensor == null)
            Debug.LogError("Please add PointerDownSensor to Gesture Recognizer!");
        
        if (particleSystem == null)
            Debug.LogWarning("Particle System of Gesture Recognizer is null, you wont know how to draw your shapes!");

        _eventStarted = false;
        _eventTimer = 0;
        _switchGesture = 0;
        _cooldownTimer = 0;
    }

    private void Start()
    {
        _downSensor.SensorTriggered.Subscribe(startAbility).AddTo(this);
        _dragSensor.SensorIsPressed.Subscribe(ActivateAbility).AddTo(this);
    }

    private void Update()
    {
        //EventTimer resets our gesture input and counts down as soon as we stop pressing
        if (_eventTimer > 0)
        {
            _eventTimer -= Time.deltaTime;
        }
        else
        {
            ResetAction();
        }

        //cooldown timer makes us wait between gestures
        if (_cooldownTimer >= 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }

    public void startAbility(EventArgs args)
    {
        ResetAction();
        _eventStarted = true;
    }

    //should honestly rather be called OnDrag or smth like that
    public void ActivateAbility(PointerEventData eventData)
    {
        if (_cooldownTimer > 0)
            return;
        
        float relativeX = eventData.position.x / Screen.width;
        float relativeY = eventData.position.y / Screen.height;

        _eventTimer = 0.1f;
        
        if (_eventStarted)
        {
            setGesturePoints(relativeX, relativeY);
            _eventStarted = false;
        }

        CheckPosition(relativeX,relativeY);
    }

    private void CheckPosition(float x , float y)
    {
        if (_pointsReached.Count != 0 && _pointsReached.Count > _point)
        {
            _pointsReached[_point] = true;
        
            for (int i = 0; i < _pointsReached.Count; i++)
            {
                _pointsReached[i] = false;
            }
        
            if (CheckBounds(x,y))
            {
                _point++;
            }
            
            if (_point >= _drawPoints.Count)
            {
                //In this Case Cast your Ability
                castAbility();
            }
        }
    }

    private void castAbility()
    {
        //Debug.Log("Cast Ability!!");
        _cooldownTimer = timeUntilNextGesture;
        StartCoroutine(StartCooldown());
        //TODO Refactor this as currently you have to expand this very manually
        switch (_switchGesture)
        {
            case 0:
                squareAbility.SetActive(true);
                break;
            case 1:
                triangleAbility.SetActive(true);
                break;
        }
        
        switchGesture();
        ResetAction();
    }
    
    private IEnumerator StartCooldown()
    {
        //_collider.enabled = false;
        yield return new WaitForSeconds(timeUntilNextGesture);
        //_collider.enabled = true;
        triangleAbility.SetActive(false);
        squareAbility.SetActive(false);
    }

    private bool CheckBounds(float x,float y)
    {   
        //check if its between x-off and x+off
        if (_drawPoints[_point].x <= x + allowedOffsetX && _drawPoints[_point].x  >= x - allowedOffsetX)
        {
            //check if its between y-off and x+off
            if (_drawPoints[_point].y <= y + allowedOffsetY && _drawPoints[_point].y >= y - allowedOffsetY)
            {
                if (!_pointsReached[_point])
                {
                    _pointsReached[_point] = true;
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    private void setGesturePoints(float x, float y)
    {
        switch (_switchGesture)
        {
            case 0:
                SetSquarePoints(x, y);
                break;
            case 1:
                SetTrianglePoints(x, y);
                break;
            default:
                SetSquarePoints(x, y);
                break;
        }
        
    }

    private void switchGesture()
    {
        switch (_switchGesture)
        {
            case 0:
                _switchGesture++;
                break;
            case 1:
                _switchGesture = 0;
                break;
            default:
                _switchGesture = 0;
                break;
        }
    }

    private void SetSquarePoints(float x,float y)
    {
        float xOff = (float)((Screen.width * 0.4) / Screen.width);
        float yOff = (float)((Screen.width * 0.4) / Screen.width);
        _drawPoints.Add(new Vector2(x, y));
        _drawPoints.Add(new Vector2(x ,y + yOff));
        _drawPoints.Add(new Vector2(x + xOff ,y + yOff));
        _drawPoints.Add(new Vector2(x + xOff ,y));
        _drawPoints.Add(new Vector2(x, y));
        
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        drawPoints();
    }
    
    private void SetTrianglePoints(float x,float y)
    {
        _drawPoints.Add(new Vector2(x, y));
        _drawPoints.Add(new Vector2(x + x / 2 ,y + y));
        _drawPoints.Add(new Vector2(x + x ,y));
        _drawPoints.Add(new Vector2(x ,y));
        
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        _pointsReached.Add(false);
        drawPoints();
    }

    private void ResetAction()
    {
        if (_abilityPoints.Count <= 0)
            return;
        
        _point = 0;
        for (int i = 0; i <= 3; i++)
        {
            _pointsReached[i] = false;
        }

        foreach (var obj in _abilityPoints)
        {
            Destroy(obj, 0.1f);
        }
        _drawPoints.Clear();
        _abilityPoints.Clear();
        _pointsReached.Clear();
    }

    private void drawPoints()
    {
        //We do our own weird translation because unity camtoworld, canvastoworld and all that didnt quite work
        Camera cam = Camera.main;
        
        double camVertical = cam.orthographicSize;
        double ratio = camVertical / cam.pixelHeight;
        double camHorizontal = cam.pixelWidth * ratio;

        Vector3 leftBot = new Vector3((float) -camHorizontal, (float) -camVertical, 0);
        Vector3 rightTop = new Vector3((float) camHorizontal * 2, (float) camVertical * 2, 0);
        
        for (int i = 0; i < _drawPoints.Count; i++)
        {
            float x = _drawPoints[i].x;
            float y = _drawPoints[i].y;
            Vector3 pos = new Vector3(leftBot.x + rightTop.x * x, leftBot.y + rightTop.y * y, 0);
            _abilityPoints.Add(Instantiate(particleSystem, pos, Quaternion.identity, enviroment.transform));
            //Debug.Log(abilityPoints.Count + " " + i);
        }
    }

}
