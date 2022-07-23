using System;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Life))]
public class Enemy : MonoBehaviour
{
    private Life _life;

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
        AudioManager.instance.PlayOneShot("Enemies");
        Destroy(gameObject);
    }
}