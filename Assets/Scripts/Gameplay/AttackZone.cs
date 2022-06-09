using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AttackZoneType
{
    Player=0,
    Enemy=5,
}

public class AttackZone : MonoBehaviour
{
    public AttackZoneType type;

    public UnityAction<PlayerCharacter> onHitPlayer;
    public UnityAction<Enemy> onHitEnemy;

    private Collider2D collide;

    void Awake()
    {
        collide = GetComponent<Collider2D>();
    }

    void Update()
    {
        
    }

    public void SetActive(bool active)
    {
        collide.enabled = active;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (type == AttackZoneType.Player && collision.GetComponent<Enemy>())
        {
            if (onHitEnemy != null)
                onHitEnemy.Invoke(collision.GetComponent<Enemy>());
        }

        if (type == AttackZoneType.Enemy && collision.GetComponent<PlayerCharacter>())
        {
            if (onHitPlayer != null)
                onHitPlayer.Invoke(collision.GetComponent<PlayerCharacter>());
        }
    }
}
