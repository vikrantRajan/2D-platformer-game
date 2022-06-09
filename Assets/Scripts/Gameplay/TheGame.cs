using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueQuests;

public class TheGame : MonoBehaviour
{
    [Header("Loader")]
    public GameObject ui_canvas;
    public GameObject audio_manager;
    [Header("Mini Games")]
    public MiniGame sweat_miniGame;
    public KeyCode BtnStartSweat;
    public MiniGame truthpick_miniGame;
    public KeyCode BtnStartTruthpick;


    public List<int> miniGameEndingList = new List<int>(); // 0 = inactive, 1 = Critical Failure, 2 = Success, 3 = Fantastic

    private bool paused = false;
    private float death_timer = 0f;
    public bool lastMiniGameSuccess = false;

    public UnityAction<bool> onPause;

    private static TheGame _instance;

    void Awake()
    {
        _instance = this;
        PlayerData.Load("player");

        if (!FindObjectOfType<TheUI>())
            Instantiate(ui_canvas);
        if (!FindObjectOfType<TheAudio>())
            Instantiate(audio_manager);
    }

    private void Start()
    {
        //Load game data
        PlayerData pdata = PlayerData.Get();
        if (!string.IsNullOrEmpty(pdata.current_scene))
        {
            if (!string.IsNullOrEmpty(pdata.current_checkpoint))
            {
                //Go to checkpoint
                Checkpoint checkpoint = Checkpoint.Get(pdata.current_checkpoint);
                if (checkpoint)
                    PlayerCharacter.Get().TeleportTo(checkpoint.transform.position);
            }
            else
            {
                //Go to entry
                LevelExit exit = LevelExit.Get(pdata.current_entry_index);
                if (exit != null)
                    PlayerCharacter.Get().TeleportTo(exit.transform.position + new Vector3(exit.entrance_offset.x, exit.entrance_offset.y));
            }
        }

        NarrativeManager.Get().onPauseGameplay += Pause;
        NarrativeManager.Get().onUnpauseGameplay += Unpause;
    }
    
    void Update()
    {
        PlayerCharacter character = PlayerCharacter.Get();
        if (character.IsDead())
        {
            death_timer += Time.deltaTime;
            if (death_timer > 2f)
                SceneNav.RestartLevel();
        }
    }

    public void Pause()
    {
        paused = true;
        if (onPause != null)
            onPause.Invoke(paused);
    }

    public void Unpause()
    {
        paused = false;
        if (onPause != null)
            onPause.Invoke(paused);
    }

    public bool IsPaused()
    {
        return paused;
    }
    
    public static TheGame Get()
    {
        return _instance;
    }
}