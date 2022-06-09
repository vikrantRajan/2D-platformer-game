using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinigameResult
{
    None=0,
    CriticalFailure=10, 
    Success=20,
    Fantastic=30,
}

public class MiniGame : MonoBehaviour
{
    [Header("Minigame")]
    public float duration = 60f;

    protected bool minigame_active = false;
    protected MinigameResult result = MinigameResult.None;
    protected float timer = 0f;

    [Header("Scoring System")]
    public int CriticalFailureScore = 10;
    public int SuccessScore = 20;
    public int FantasticScore = 30;

    protected int score = 0;
    protected int lose_score = 0;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (!minigame_active)
            return;

        //Debug.Log("Minigame Started");

        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f);
        timer += Time.deltaTime;
        
    }

    public virtual void StartMinigame()
    {
        if (!minigame_active)
        {
            minigame_active = true;
            timer = 0f;
            score = 0;
            TheGame.Get().Pause();
            gameObject.SetActive(true);
            Debug.Log("Start Minigame " + gameObject.name);
        }
    }

    public virtual void EndMinigame(MinigameResult result)
    {
        if (minigame_active)
        {
            minigame_active = false;
            this.result = result;
            TheGame.Get().Unpause();
            gameObject.SetActive(false);
        }
    }

    public bool IsActive()
    {
        return minigame_active;
    }

    public bool IsCompleted()
    {
        return result != MinigameResult.None;
    }

    public void AddPoint()
    {
        score += 1;
        //Debug.Log("SCORE: " + score);
    }

    public void LosePoint()
    {
        lose_score += 1;
        //Debug.Log("Lose Score: " + lose_score);
    }

    public int GetScore()
    {
        return score; 
    }

    public int GetLoseScore()
    {
        return lose_score;
    }

}
