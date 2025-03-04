using System;
using System.Collections;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Sensor))]
[RequireComponent(typeof(Collider2D))]
public class PushBackHeroAbility : MonoBehaviour
{
	private Collider2D _collider;
	private Sensor activationCause;
	
	//TODO maybe make the key Access here better, as its just a String and you could forget where you need to change it (Ability Script, GameData and AbilityIcon)
	private string abilityNameKey = "PushBack";
	
	[SerializeField] private float duration = 0.5f;
	[SerializeField] private float strength = 1f;
	[SerializeField] AnimationCurve speedChangeCurve;

	[Space(5)]
	[SerializeField] private float cooldownDuration = 4;


	[Space(5)]
	[SerializeField] private GameObject abilityVFX;

	private void Start()
	{
		_collider = GetComponent<Collider2D>();
		activationCause = GetComponent<Sensor>();
		activationCause.SensorTriggered.Subscribe(ActivateAbility).AddTo(this);

		StartCoroutine(StartCooldown());
	}

	public void ActivateAbility(EventArgs args)
	{
		if (PauseManager.Instance.isPaused.Value)
			return;
		
		
		if (GameData.Instance.GetAbilityAvailable(abilityNameKey))
		{
			Debug.Log("PushBack works");
			// To the future people: Implement a nicer way to access the enemies here.
			AudioManager.Instance.Play("PushbackAbility");
			EnemyMovement[] allEnemies = Array.ConvertAll(GameObject.FindGameObjectsWithTag("Enemy"), (go) => go.GetComponent<EnemyMovement>());
			for (int i = 0; i < allEnemies.Length; i++)
			{
				allEnemies[i].PushBack(duration, strength, speedChangeCurve);
			}

			abilityVFX.SetActive(true);
			StartCoroutine(StartCooldown());
		}
	}

	private IEnumerator StartCooldown()
	{
		GameData.Instance.SetAbilityAvailable(abilityNameKey, false);
		_collider.enabled = false;
		yield return new WaitForSeconds(cooldownDuration);
		GameData.Instance.SetAbilityAvailable(abilityNameKey, true);
		_collider.enabled = true;
	}
}
