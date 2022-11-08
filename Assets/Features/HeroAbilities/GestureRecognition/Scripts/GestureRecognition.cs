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
    //TODO refactor all switch cases here for future easier expansion
    //TODO add double Tap to the Gesture Recognition, and put it all together into one thing for Ability Activation
    //If you have to work with this in the future, im sorry, gesture recognition in this way might not be the best approach
    
    public GameObject shadowAbility;
    public GameObject morningStarAbility;

    public GameObject morningStarGUI;
    public GameObject shadowCloneGUI;
    private AbilityIcon morningStarAbilityIcon;
    private AbilityIcon shadowCloneAbilityIcon;
    
    private PointerDragSensor _dragSensor;
    private PointerDownSensor _downSensor;
    private PointerUpSensor _upSensor;
    private BoxCollider2D _collider;

    public GameObject particleSystem;
    public GameObject enviroment;

    [SerializeField] private float allowedOffsetX = 0.1f;
    [SerializeField] private float allowedOffsetY = 0.1f;
    [SerializeField] private float timeBetweenGestures = 0.1f;
    [SerializeField] private float abilityTime = 15f;
    [SerializeField] private float timeUntilNextGesture = 0f;

    private List<Vector2> _drawPoints = new List<Vector2>();
    private bool _actionStarted = false;
    private int _point = 0;
    private List<bool> _pointsReached = new List<bool>();

    private bool _eventStarted;
    private float _eventTimer;

    
    private float _cooldownTimer;

    private int _switchGesture;

    private List<GameObject> _abilityPoints = new List<GameObject>();

    private Coroutine dragCooldownCoroutine;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.enabled = true;
        _dragSensor = GetComponent<PointerDragSensor>();
        _downSensor = GetComponent<PointerDownSensor>();
        _upSensor = GetComponent<PointerUpSensor>();

        _eventStarted = false;
        _eventTimer = 0;
        _switchGesture = 0;
        _cooldownTimer = timeUntilNextGesture;

        morningStarAbilityIcon = morningStarGUI.GetComponent<AbilityIcon>();
        shadowCloneAbilityIcon = shadowCloneGUI.GetComponent<AbilityIcon>();
        
        SafetyChecks();
    }

    /// <summary>
    /// Check if the manual setup for the gesture recognition is actually set up
    /// </summary>
    private void SafetyChecks()
    {
        if(_collider == null)
            Debug.LogError("Gesture Recognition needs a Collider to function!");
        
        if(_dragSensor == null)
            Debug.LogError("Please add PointerDragSensor to Gesture Recognizer!");
        
        if(_downSensor == null)
            Debug.LogError("Please add PointerDownSensor to Gesture Recognizer!");
        
        if (particleSystem == null)
            Debug.LogWarning("Particle System of Gesture Recognizer is null, you wont know how to draw your shapes!");
        
        if (morningStarAbilityIcon == null)
        {
            Debug.LogWarning("Couldnt find AbilityIcon Script for MorningStar GUI-Object");
        }
        
        if (shadowCloneAbilityIcon == null)
        {
            Debug.LogWarning("Couldnt find AbilityIcon Script for ShadowAbility GUI-Object");
        }
    }

    /// <summary>
    /// we setup our sensors so we activate our ability when dragging and otherwise start a cooldown coroutine so we cant spam the gesture recognition
    /// </summary>
    private void Start()
    {
        _downSensor.SensorTriggered.Subscribe(e =>
        {
            dragCooldownCoroutine = StartCoroutine(DragCooldown(e, timeBetweenGestures));
        }).AddTo(this);
        _dragSensor.SensorIsPressed.Subscribe(ActivateAbility).AddTo(this);
        _upSensor.SensorTriggered.Subscribe(e =>
        {
            StopCoroutine(dragCooldownCoroutine);
        }).AddTo(this);
        
        SetupGUI();
    }
    
    /// <summary>
    /// Here we Setup and change the GUI Icons for our Gesture Abilities
    /// </summary>
    private void SetupGUI()
    {
        switch (_switchGesture)
        {
            case 0:
                morningStarGUI.SetActive(true);
                shadowCloneGUI.SetActive(false);
                morningStarAbilityIcon.SetIcon(false);
                break;
            case 1:
                morningStarGUI.SetActive(false);
                shadowCloneGUI.SetActive(true);
                shadowCloneAbilityIcon.SetIcon(false);
                break;
        }
    }
    
    /// <summary>
    /// If our ability is ready to be cast we want the icon to represent that
    /// </summary>
    private void AbilityIconReady()
    {
        switch (_switchGesture)
        {
            case 0:
                morningStarAbilityIcon.SetIcon(true);
                break;
            case 1:
                shadowCloneAbilityIcon.SetIcon(true);
                break;
        }
    }

    private IEnumerator DragCooldown(EventArgs e, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        StartAbility(e);
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
            
            //in case the cooldown timer ran out we can do stuff here, once after it ran out
            if (_cooldownTimer <= 0)
            {
                AbilityIconReady();
            }
        }
    }

    public void StartAbility(EventArgs args)
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
            SetGesturePoints(relativeX, relativeY);
            _eventStarted = false;
        }

        CheckPosition(relativeX,relativeY);
    }

    /// <summary>
    /// Check position checks, for a given x and y, if it equals the next point you have to draw for your current gestures, this sadly also means it only works if you draw the gesture the right way around
    /// </summary>
    /// <param name="x">your drag position x to compare to the next point</param>
    /// <param name="y">your drag position y to compare to the next point</param>
    private void CheckPosition(float x , float y)
    {
        if (_pointsReached.Count != 0 && _pointsReached.Count > _point)
        {
            _pointsReached[_point] = true;
        
            for (int i = 0; i < _pointsReached.Count; i++)
            {
                _pointsReached[i] = false;
            }
        
            if (CheckBounds(x,y,_drawPoints[_point]))
            {
                _point++;
            }
            
            if (_point >= _drawPoints.Count)
            {
                //In this Case Cast your Ability
                CastAbility();
            }
        }
    }
    
    /// <summary>
    /// syntactic sugar for checkposition, this checks if the given x and y are within a radius of our current point
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool CheckBounds(float x,float y, Vector2 point)
    {   
        //check if its between x-off and x+off
        if (point.x <= x + allowedOffsetX && point.x  >= x - allowedOffsetX)
        {
            //check if its between y-off and x+off
            if (point.y <= y + allowedOffsetY && point.y >= y - allowedOffsetY)
            {
                //git blame myself? idk why we do this but it seems to have some reason
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

    /// <summary>
    /// Here we actually cast our ability
    /// </summary>
    private void CastAbility()
    {
        // We make the player wait the cooldown + the time the ability is active
        
        _cooldownTimer = timeUntilNextGesture + abilityTime;
        StartCoroutine(StartCooldown());
        
        //TODO Refactor this as currently you have to expand this very manually
        switch (_switchGesture)
        {
            case 0:
                morningStarAbility.SetActive(true);
                break;
            case 1:
                
                shadowAbility.SetActive(true);
                break;
        }
        
        //For special cases that instant trigger we want to setup the GUI right away and make the cooldown smaller
        if (_switchGesture == 1)
        {
            //In this case the player doesnt have to wait until the ability is over as it is an instant cast
            _cooldownTimer = timeUntilNextGesture;
            //We also instantly switch and setup the GUI in this case as we dont want to wait
            SwitchGesture();
            SetupGUI();
        }
        else
        {
            SwitchGesture();
        }
        
        
        ResetAction();
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
    
    private IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(abilityTime);
        shadowAbility.SetActive(false);
        morningStarAbility.SetActive(false);
        
        //We setup the GUI again as we want it to switch for the current case
        SetupGUI();
    }

    private void SetGesturePoints(float x, float y)
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

    /// <summary>
    /// Switching between different gestures, right now we just circle through them, between the first and the second one
    /// </summary>
    private void SwitchGesture()
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

    /// <summary>
    /// For a given coordinate we setup a rectangle of points from it as an origin
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
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
        Invoke(nameof(DrawPoints), 0.01f);
    }
    
    /// <summary>
    /// For a given coordinate we setup a triangle of points from it as an origin
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
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
        Invoke(nameof(DrawPoints), 0.01f);
    }

    /// <summary>
    /// Here we actually draw the points as objects into our world, this is weirdly complicated because of our input system (i think?)
    /// </summary>
    private void DrawPoints()
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
