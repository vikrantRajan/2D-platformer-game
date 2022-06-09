using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDefeatMusic : MonoBehaviour
{
    public AudioSource deathMusic;
    public float length_of_music = 5f;
    [Range(0f, 1f)] public float max_volume = 1f;
    private float curT = 0;
    public BackgroundMusicController backgroundMusicController;
    public Enemy enemy;
    private bool playedOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        curT = 0;
        deathMusic.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.IsDead() && !playedOnce)
        {
            backgroundMusicController.StopAllAudio();
            StartCoroutine(StartFade(deathMusic, 1f, max_volume));
            deathMusic.Play();
            playedOnce = true;
        }

        if (playedOnce)
        {
            curT += Time.deltaTime;
            if (curT > length_of_music)
            {
                deathMusic.Stop();
            }
        }

        if (!deathMusic.isPlaying && playedOnce)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        yield break;
    }
}
