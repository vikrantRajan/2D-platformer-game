using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : UIPanel
{
    public SliderUI master_volume;
    public SliderUI music_volume;
    public SliderUI sfx_volume;
    public ChoiceUI resolution_choice;
    public ChoiceUI fullscreen_choice;

    private bool refreshing = false;
    private List<Resolution> resolutions = new List<Resolution>();

    private static OptionsPanel _instance;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;

        master_volume.onChange += OnChangeMasterVol;
        music_volume.onChange += OnChangeMusicVol;
        sfx_volume.onChange += OnChangeSfxVol;
        resolution_choice.onChange += OnChangeResolution;
        fullscreen_choice.onChange += OnChangeDisplay;
    }

    private void RefreshPanel()
    {
        resolutions.Clear();
        resolution_choice.ResetChoice();

        Resolution[] tempresolutions = Screen.resolutions;
        int x = Screen.currentResolution.width;
        int y = Screen.currentResolution.height;
        refreshing = true;

        HashSet<string> rez_taken = new HashSet<string>();
        System.Array.Reverse(tempresolutions);
        foreach (Resolution reso in tempresolutions)
        {
            string rez_tag = reso.width + "x" + reso.height;
            if (!rez_taken.Contains(rez_tag))
            {
                rez_taken.Add(rez_tag);
                resolutions.Add(reso);
                resolution_choice.AddChoice(rez_tag, reso.width + " x " + reso.height);
                if (reso.width == x && reso.height == y)
                    resolution_choice.SetTo(rez_tag);
            }
        }

        if (resolution_choice.GetSelectedIndex() < 0)
            resolution_choice.SetTo(0);

        fullscreen_choice.ResetChoice();
        fullscreen_choice.AddChoice("fullscreen", "FULLSCREEN");
        fullscreen_choice.AddChoice("windowed", "WINDOWED");
        fullscreen_choice.SetTo(Screen.fullScreen ? 0 : 1);

        master_volume.SetValue(PlayerData.Get().master_volume * 100f);
        music_volume.SetValue(PlayerData.Get().music_volume * 100f);
        sfx_volume.SetValue(PlayerData.Get().sfx_volume * 100f);
        
        refreshing = false;
    }

    private void OnChangeMasterVol(float value)
    {
        float vol = master_volume.GetValue();
        PlayerData.Get().master_volume = vol / 100f;
        TheAudio.Get().RefreshVolume();
    }

    private void OnChangeSfxVol(float value)
    {
        float vol = sfx_volume.GetValue();
        PlayerData.Get().sfx_volume = vol / 100f;
        TheAudio.Get().RefreshVolume();
    }

    private void OnChangeMusicVol(float value)
    {
        float vol = music_volume.GetValue();
        PlayerData.Get().music_volume = vol / 100f;
        TheAudio.Get().RefreshVolume();
    }

    private void OnChangeDisplay(string value)
    {
        if (refreshing)
            return;

        FullScreenMode mode = fullscreen_choice.GetSelectedValue() == "fullscreen" ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Resolution reso = resolutions[resolution_choice.GetSelectedIndex()];
        Screen.SetResolution(reso.width, reso.height, mode);

        PlayerPrefs.SetInt("fullscreen", mode == FullScreenMode.FullScreenWindow ? 1 : 0);
    }

    private void OnChangeResolution(string value)
    {
        if (refreshing)
            return;

        FullScreenMode mode = fullscreen_choice.GetSelectedValue()  == "fullscreen" ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Resolution reso = resolutions[resolution_choice.GetSelectedIndex()];
        Screen.SetResolution(reso.width, reso.height, mode);

        PlayerPrefs.SetInt("resolution_x", reso.width);
        PlayerPrefs.SetInt("resolution_y", reso.height);
    }

    public void CloseAndSave()
    {
        PlayerData.Get().Save();
        Hide();
    }

    public override void Show(bool instant = false)
    {
        base.Show(instant);
        TheGame.Get().Pause();
        RefreshPanel();
    }

    public override void Hide(bool instant = false)
    {
        base.Hide(instant);
        TheGame.Get().Unpause();
        PausePanel.Get().Show();
    }

    public static OptionsPanel Get()
    {
        return _instance;

    }
}
