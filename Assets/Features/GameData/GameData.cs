using System;
using UnityEngine;
using DyrdaDev.Singleton;
using UniRx;
using System.Collections.Generic;
using Random = System.Random;

public class GameData : SingletonMonoBehaviour<GameData>
{
    public enum LevelTheme
    {
        Demons,
        Undeads,
        Orcs
    }

    public ReactiveProperty<int> score = new ReactiveProperty<int>(0);
    
    //TODO for future developers, reformat this and make it nicer for more abiliities
    private Dictionary<String, ReactiveProperty<bool>> abilitiesAvailable =
        new Dictionary<string, ReactiveProperty<bool>>();

    private ReactiveProperty<bool> pushBackAbilityAvailable = new ReactiveProperty<bool>(false);
    private ReactiveProperty<bool> morningStarAbilityAvailable = new ReactiveProperty<bool>(false);
    private ReactiveProperty<bool> shadowAbilityAvailable = new ReactiveProperty<bool>(false);

    [HideInInspector] public LevelTheme currentLevelTheme;
    private Random LevelThemeRandom = new Random();

    public void Awake()
    {
        abilitiesAvailable.Add("PushBack", pushBackAbilityAvailable);
        abilitiesAvailable.Add("MorningStar", morningStarAbilityAvailable);
        abilitiesAvailable.Add("Shadow", shadowAbilityAvailable);
        currentLevelTheme = GetRandomLevelTheme();
    }

    public void IncreaseScore(int value)
    {
        score.Value += value;
    }

    public void ResetScore()
    {
        score.Value = 0;
    }

    public void SetAbilityAvailable(string abilityName, bool value)
    {
        if (!abilitiesAvailable.ContainsKey(abilityName))
        {
            Debug.LogError(abilityName + " cant be set to " + value + " as the key doesnt exist. Add the necessary key and ReactiveProperty to GameData.");
        }
        
        abilitiesAvailable[abilityName].Value = value;
    }
    
    public bool GetAbilityAvailable(string abilityName)
    {
        if (!abilitiesAvailable.ContainsKey(abilityName))
        {
            Debug.LogError(abilityName + " cant be found as the key doesnt exist. Add the necessary key and ReactiveProperty to GameData.");
        }

        return abilitiesAvailable[abilityName].Value;
    }
    
    public ReactiveProperty<bool> GetAbilityAvailableReactiveProperty(string abilityName)
    {
        if (!abilitiesAvailable.ContainsKey(abilityName))
        {
            Debug.LogError(abilityName + " cant be found as the key doesnt exist. Add the necessary key and ReactiveProperty to GameData.");
        }

        return abilitiesAvailable[abilityName];
    }


    private void GetNextLevelTheme()
    {
        var previousLevelTheme = currentLevelTheme;
        var newLevelTheme = currentLevelTheme;
        
        while (previousLevelTheme == newLevelTheme)
        {
            newLevelTheme = GetRandomLevelTheme();
        }

        currentLevelTheme = newLevelTheme;
    }


    public void Reset()
    {
        ResetScore();
        GetNextLevelTheme();
    }

    private LevelTheme GetRandomLevelTheme()
    {
        Array values = Enum.GetValues(typeof(LevelTheme));
        return (LevelTheme)values.GetValue(LevelThemeRandom.Next(values.Length));
    }
}