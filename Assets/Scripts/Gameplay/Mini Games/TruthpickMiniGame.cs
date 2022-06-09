using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruthpickMiniGame : MiniGame
{
    private static TruthpickMiniGame instance;
    //public float MaxLoseScore = 5f;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void Update()
    {
        base.Update();
        if (timer > duration) CheckScore();
         else if (GetScore() > 0) CheckScore();  // This means the tooth pick has been picked up
        
    }

    private void CheckScore()
    {
        // in this case we want to check the number of times pricked
        // 0 pricks = fantastic
        // 1+ pricks = success
        // 5+ pricks = critical failure

        if (GetLoseScore() >= CriticalFailureScore)
        {
            EndMinigame(MinigameResult.CriticalFailure);
        }

        if (GetLoseScore() < CriticalFailureScore && GetLoseScore() > FantasticScore)
        {
            EndMinigame(MinigameResult.Success);
        }

        if (GetLoseScore() <= FantasticScore)
        {
            EndMinigame(MinigameResult.Fantastic);
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

    public static TruthpickMiniGame Get()
    {
        return instance;
    }
}
