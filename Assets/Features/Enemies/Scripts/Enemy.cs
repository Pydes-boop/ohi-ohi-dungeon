using System;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Life))]
public class Enemy : MonoBehaviour
{
    private Life _life;
    public AudioClip[] deathSounds;

    private void OnEnable()
    {
        _life = GetComponent<Life>();
        _life.Defeated += EnemyDefeated;
    }

    private void OnDisable()
    {
        _life.Defeated -= EnemyDefeated;
    }
    
    public void EnemyDefeated(object sender, EventArgs args) {
        AudioManager.instance.ChangePitch("Enemies", Random.Range(.7f, 1.3f));
        AudioManager.instance.PlayOneShot("Enemies", deathSounds[Random.Range(0, deathSounds.Length)]);
        Destroy(gameObject);
    }
}