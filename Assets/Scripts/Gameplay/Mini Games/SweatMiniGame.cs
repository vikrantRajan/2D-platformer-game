using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatMiniGame : MiniGame
{

    public float MaxLostSweatDrops = 5f;


    private static SweatMiniGame instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Update()
    {
        base.Update();
        if (timer > duration)
        {
            CheckScore();
        }

    }

    public override void StartMinigame()
    {
        base.StartMinigame();

    }

    public override void EndMinigame(MinigameResult result)
    {
        base.EndMinigame(result);
    }

    private void CheckScore()
    {

        if (GetScore() <= CriticalFailureScore || GetLoseScore() >= MaxLostSweatDrops)
        {
            EndMinigame(MinigameResult.CriticalFailure);
        }

        if (GetScore() > SuccessScore && GetScore() < FantasticScore)
        {
            EndMinigame(MinigameResult.Success);
        }

        if (GetScore() >= FantasticScore)
        {
            EndMinigame(MinigameResult.Fantastic);
        }
    }

    public static SweatMiniGame Get()
    {
        return instance;
    }
}
