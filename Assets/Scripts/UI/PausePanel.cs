using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIPanel
{


    private static PausePanel _instance;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
    }

    public void OnClickResume()
    {
        Hide();
    }

    public void OnClickOptions()
    {
        Hide();
        OptionsPanel.Get().Show();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public override void Show(bool instant = false)
    {
        base.Show(instant);
        TheGame.Get().Pause();
    }

    public override void Hide(bool instant = false)
    {
        base.Hide(instant);
        TheGame.Get().Unpause();
    }

    public static PausePanel Get()
    {
        return _instance;

    }
}
