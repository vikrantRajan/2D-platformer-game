using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackgroundMusicController : MonoBehaviour
{
    [Serializable]
    public struct BackgroundMusicTracks
    {
        public AudioSource bgMusic;
    }
    public PlayerCharacter playerCharacter;
    private bool isPlaying = false;
    public AudioSource playerDeathAudio;
    public BackgroundMusicTracks[] backgroundMusic;
    private GameObject prev_controller_game_object;
    private bool previousControllerExists = false;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameObject[] controllers  = GameObject.FindGameObjectsWithTag("Music Controller");
        Debug.Log("CONTROLLERS "+controllers.Length);


        if (controllers.Length > 1)
        {
            previousControllerExists = true;
            prev_controller_game_object = GameObject.FindGameObjectsWithTag("Music Controller")[0].gameObject;
        }

    }

    public void DestroyPrevController(GameObject x)
    {
        Destroy(x);
    }

    public GameObject GetPrevControllerGameObject()
    {
        return prev_controller_game_object;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerCharacter.IsDead())
        {
            StopAllAudio();
            if(!playerDeathAudio.isPlaying) playerDeathAudio.Play();
            Destroy(gameObject);
        }
    }

    public void StopAllAudio()
    {
        foreach (BackgroundMusicTracks x in backgroundMusic)
        {
            x.bgMusic.Stop();
        }
    }

    public bool MusicColliderPlaying()
    {
        return isPlaying;
    }

    public void PlayingStatus(bool x)
    {
        isPlaying = x;
    }

    public bool PreviousControllerExists()
    {
        return previousControllerExists;
        
    }

}
