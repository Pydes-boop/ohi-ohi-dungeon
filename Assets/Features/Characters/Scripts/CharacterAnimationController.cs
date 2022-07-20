using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private Animator _animator;
    public bool alwaysRunning;
    public AudioClip stepSFX;
    private Vector3 lastPosition;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        lastPosition = transform.position;
        StartCoroutine(stepsWhileWalkingOnScene());
    }

    private void Update()
    {
        if (transform.position != lastPosition || alwaysRunning)
        {
            _animator.SetBool("isFollowing", true);
            lastPosition = transform.position;
        }
        else
        {
            _animator.SetBool("isFollowing", false);
        }
    }
    
    IEnumerator stepsWhileWalkingOnScene() {
        int stepsTotal = 8;
        int stepsCount = 0;
        AudioManager.instance.ChangePan("Steps", 1);
        
        while (stepsCount < stepsTotal) {
            AudioManager.instance.ChangePanRelative("Steps", -(1/(float)stepsTotal));
            
            if(stepSFX != null)
                AudioManager.instance.PlayOneShot("Steps", stepSFX);
            
            stepsCount++;
            yield return new WaitForSeconds(.4f);
        }
    }
}