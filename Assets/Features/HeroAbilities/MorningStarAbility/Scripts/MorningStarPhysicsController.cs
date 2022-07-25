using System.Collections;
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

    [Header("Audio")] 
    [SerializeField] private string soundName;
    [SerializeField] private float soundCooldown;
    [SerializeField] private AnimationCurve PitchCurve;

    private Rigidbody2D _rigidbody;
    private float _oldDrag;

    private bool _dragged = false;

    private Coroutine _soundCoroutine;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _oldDrag = _rigidbody.drag;
    }

    private void Start()
    {
        ObservableDragTrigger observable = hitBox.TryGetComponent(typeof(ObservableDragTrigger), out var component) ?
            (ObservableDragTrigger)component : hitBox.AddComponent<ObservableDragTrigger>();
        observable.OnDragAsObservable().Subscribe(OnDrag).AddTo(this);
        
        hitBox.GetComponent<PointerDownSensor>().SensorTriggered.Subscribe(_ =>
            {
                _dragged = false;
                _rigidbody.drag = _oldDrag;
            })
            .AddTo(this);
        hitBox.AddComponent<ObservablePointerUpTrigger>().OnPointerUpAsObservable().Subscribe(_ => _dragged = true)
            .AddTo(this);
    }

    private void FixedUpdate()
    {
        if (_dragged)
            return;
        
        if (_rigidbody.velocity.sqrMagnitude < groundingVelocity * groundingVelocity)
        {
            _rigidbody.drag = groundDrag;
            _dragged = true;
        }
    }

    private void OnDrag(PointerEventData data)
    {
        _rigidbody.AddForce((_rigidbody.position - (Vector2)stick.transform.position).normalized * force * Time.deltaTime);
        AudioManager.Instance.ChangePitch(soundName, PitchCurve.Evaluate(evaluatePitchCurve()));
        AudioManager.Instance.ChangeVolume(soundName, PitchCurve.Evaluate(evaluatePitchCurve()));
    }

    private IEnumerator SoundPlayer()
    {
        while (true)
        {
            //using some magic here to get some nice numbers for pitch and volume
            AudioManager.Instance.Play(soundName);
            yield return new WaitForSeconds(soundCooldown);
        }
    }

    private void OnEnable()
    {
        _soundCoroutine = StartCoroutine(SoundPlayer());
        AudioManager.Instance.ChangePitch(soundName, evaluatePitchCurve());
        AudioManager.Instance.ChangeVolume(soundName, evaluatePitchCurve());
    }

    private void OnDisable()
    {
        StopCoroutine(_soundCoroutine);
    }

    private float evaluatePitchCurve()
    {
        return PitchCurve.Evaluate(_rigidbody.velocity.magnitude / 35);
    }
}
