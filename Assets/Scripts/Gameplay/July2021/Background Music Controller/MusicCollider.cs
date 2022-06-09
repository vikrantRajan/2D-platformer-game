using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCollider : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public BackgroundMusicController backgroundMusicController;
    private int triggerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        triggerCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player" && triggerCount == 0)
        {
            if(backgroundMusicController.PreviousControllerExists())
            {
                backgroundMusicController.DestroyPrevController(backgroundMusicController.GetPrevControllerGameObject());
            }
            triggerCount++;
            backgroundMusicController.StopAllAudio();
            backgroundMusic.Play();
            backgroundMusicController.PlayingStatus(true);
        }
    }

}
