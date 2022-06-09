using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    private Enemy enemy;
    private RangedEnemy rangedEnemy;
    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
        enemy = gameObject.transform.parent.gameObject.GetComponent<Enemy>();
        rangedEnemy = gameObject.transform.parent.gameObject.GetComponent<RangedEnemy>();
        Destroy(gameObject, rangedEnemy.destroy_arrow_timer);

    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsLeft();
    }

    private void MoveTowardsLeft()
    {
        gameObject.transform.Translate(Vector3.left * Time.deltaTime * rangedEnemy.arrowSpeed);
        if (rangedEnemy.GetShootDirection()) gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.tag == "Player")
        {
            playerCharacter.TakeDamage(enemy.attack_damage);
            Destroy(gameObject);
        }
    }
}
