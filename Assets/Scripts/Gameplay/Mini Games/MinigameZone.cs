using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameZone : MonoBehaviour
{
    public MiniGame minigame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>())
        {
            if (minigame != null && !minigame.IsActive() && !minigame.IsCompleted())
            {
                minigame.StartMinigame();
            }
        }

    }
}
