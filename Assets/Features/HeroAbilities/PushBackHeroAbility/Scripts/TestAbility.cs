using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Drawing;
using UnityEngine.Assertions.Comparers;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Sensor))]
public class TestAbility : MonoBehaviour
{
    public Canvas canvas;
    
    private Sensor _activationCause;
    private Collider2D _collider;

    [SerializeField] private float allowedOffsetX = 0.1f;
    [SerializeField] private float allowedOffsetY = 0.1f;

    private float _startTime;
    private Vector2[] _squarePoints = new Vector2[4];
    private bool _actionStarted = false;
    private int point = 0;
    private bool[] pointsReached = new bool[4];

    private float _lastCall = 0.0f;

    private void Awake()
    {
        //initialize it to false
        for (int i = 0; i <= 3; i++)
        {
            pointsReached[i] = false;
        }
    }

    private void Start()
    {

        _collider = GetComponent<Collider2D>();
        _collider.enabled = true;
        _activationCause = GetComponent<Sensor>();
        _activationCause.SensorIsPressed.Subscribe(ActivateAbility).AddTo(this);

    }

    //should honestly rather be called OnDrag or smth like that
    public void ActivateAbility(PointerEventData eventData)
    {
        //limit succesive drags so that if its in the same position in succesive frames u dont get multiple 
        //checkpoints checkd (DIRTY) alternative 1: check for position instead of time
        //                           alternative 2: change how checkboundaries work maybe bool[] so everything needs to be true
        if (_lastCall + 0.25f < Time.time)
        {
            float relativeX = eventData.position.x / Screen.width;
            float relativeY = eventData.position.y / Screen.height;
        
            if (!_actionStarted)
            {
                Debug.Log("PosX:" + eventData.position.x / Screen.width);
                Debug.Log("PosY:" + eventData.position.y / Screen.height);
                //initialize it to false
                for (int i = 0; i <= 3; i++)
                {
                    pointsReached[i] = false;
                }
            }

            
            if (point > 3)
            {
                Debug.Log("Cast Ability!!");
                point = 0;
                _actionStarted = false;
                //initialize it to false
                for (int i = 0; i <= 3; i++)
                {
                    pointsReached[i] = false;
                }
                //Start Cooldown
            }
            CheckPosition(relativeX,relativeY);
        }
        else
        {
            _lastCall = Time.time;
            point = 0;
            _actionStarted = false;
            //initialize it to false
            for (int i = 0; i <= 3; i++)
            {
                pointsReached[i] = false;
            }
        }
        
    }

    private void CheckPosition(float x , float y)
    {
        //Debug.Log("CurrentTime: " + Time.time + "StartTime" + _startTime);
        
        //needs to potentionally go to update function or need to be IENUMERATOR
        if (Time.time >= _startTime + 50.5f)
        {
            Debug.Log("test");
            for (int i = 0; i < 4; i++)
            {
                Debug.Log("Point " + i +": x " +_squarePoints[i].x + " Y: "+ _squarePoints[i].y);
            } 

            Debug.Log("Time is up");
            _actionStarted = false;
            point = 0;
            for (int i = 0; i <= 3; i++)
            {
                pointsReached[i] = false;
            }
        }
        
        if (!_actionStarted)
        {
            pointsReached[point] = true;
            SetSquarePoints(x,y);
            for (int i = 0; i <= 3; i++)
            {
                pointsReached[i] = false;
            }
            _actionStarted = true;
        }

        //Debug.Log("Checking Bounds-------------------" + "Point: " + point);
        if (CheckBounds(x,y))
        {
            point++;
            Debug.Log("Point: " + point);
        }

    }

    private bool CheckBounds(float x,float y)
    {   
        //check if its between x-off and x+off
        if (_squarePoints[point].x <= x + allowedOffsetX && _squarePoints[point].x  >= x - allowedOffsetX)
        {
            //check if its between y-off and x+off
            if (_squarePoints[point].y <= y + allowedOffsetY && _squarePoints[point].y >= y - allowedOffsetY)
            {
                Debug.Log("expected x: " + _squarePoints[point].x + "actual: " + x);
                Debug.Log("expected y: " + _squarePoints[point].y + "actual: " + y);
                //only return true if its at a new position
                Debug.Log("Point: " + point + " "+pointsReached[point]);
                if (!pointsReached[point])
                {
                    Debug.Log("Test");
                    pointsReached[point] = true;
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    private void SetSquarePoints(float x,float y)
    {
        float xOff = (float)((Screen.width * 0.4) / Screen.width);
        float yOff = (float)((Screen.width * 0.4) / Screen.width);
        _squarePoints[0] = new Vector2(x, y);
        _squarePoints[1] = new Vector2(x ,y + yOff);
        _squarePoints[2] = new Vector2(x + xOff ,y + yOff);
        _squarePoints[3] = new Vector2(x + xOff ,y);
        
        
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("Point " + i +": x " +_squarePoints[i].x + " Y: "+ _squarePoints[i].y);
        }

        _startTime = Time.time;
        //DrawPoints();
    }

    private void DrawPoints()
    {
        GameObject img1 = new GameObject("Point 1");
        RectTransform rect1 = img1.AddComponent<RectTransform>();
        rect1.transform.SetParent(canvas.transform);
        rect1.sizeDelta = new Vector2((float)(0.05 * Screen.width), (float)(0.05 * Screen.height));
        Image image = img1.AddComponent<Image>();
        //Texture2D tex = Resources.Load<Texture2D>("red");
        img1.transform.SetParent(canvas.transform);
    }

    

}
