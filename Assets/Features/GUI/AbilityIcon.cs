using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Image))]
public class AbilityIcon : MonoBehaviour
{
    private Image icon;

    [SerializeField] private string abilityName;

    [SerializeField] private float onCooldownAlpha = 100;
    
    [SerializeField] private Text tutorialTextObject;
    [SerializeField] private string tutorialTextString;
    
    private bool oneTimePause;

    private void Start()
    {
        oneTimePause = true;
        icon = GetComponent<Image>();

        GameData.Instance.GetAbilityAvailableReactiveProperty(abilityName).Subscribe(value => SetIcon(value))
            .AddTo(this);
    }

    public void SetIcon(bool value)
    {
        Color c = icon.color;
        c.a = value ? 1 : onCooldownAlpha / 255;
        icon.color = c;
    }

    public void pauseAndExplain()
    {
        if (oneTimePause)
        {
            oneTimePause = false;
            PauseManager.Instance.isPaused.Value = true;
            tutorialTextObject.text = tutorialTextString;

            //TODO add explanation Text and animation?
        }
    }
}
