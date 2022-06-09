using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheUI : MonoBehaviour
{
    public ProgressBar hp;
    public ProgressBar mana;

    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        
    }

    private void Start()
    {
        canvas.worldCamera = TheCamera.Get().GetCamera();
    }

    void Update()
    {
        PlayerCharacter character = PlayerCharacter.Get();
        hp.SetValue(character.GetHP());
        hp.SetMax(character.hp_max);
        mana.SetValue(character.GetMana());
        mana.SetMax(character.mana_max);

        if (PlayerControls.Get().IsPause())
        {
            if (OptionsPanel.Get().IsVisible())
                OptionsPanel.Get().CloseAndSave();
            else
                PausePanel.Get().Toggle();
        }
    }
}
