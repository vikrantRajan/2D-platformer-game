using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatLoseZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "SweatDrop(Clone)" || collision.gameObject.name == "SweatDrop")
        {
            Destroy(collision.gameObject);
            SweatMiniGame.Get().LosePoint();
        }

    }
}
