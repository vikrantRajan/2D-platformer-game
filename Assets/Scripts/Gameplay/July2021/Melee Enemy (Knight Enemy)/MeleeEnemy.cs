using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public Transform playerDetection;

    private float distance = 1f;
    private Animator anim;
    private bool attackPlayer = false;
    private Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemy.IsDead())
        {
            WalkAndWaitToAttack();
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        if (attackPlayer)
        {
            enemy.StopMoving();
            anim.SetTrigger("Attack");
            RaycastHit2D detectLeft = Physics2D.Raycast(playerDetection.position, Vector2.left, distance);
            RaycastHit2D detectRight = Physics2D.Raycast(playerDetection.position, Vector2.right, distance);
            bool AttackPlayerLeft = true;
            bool AttackPlayerRight = true;
            if (!detectRight.collider || detectRight.collider.gameObject.tag != "Player")
            {
                AttackPlayerRight = false;
            }

            if (!detectLeft.collider || detectLeft.collider.gameObject.tag != "Player")
            {
                AttackPlayerLeft = false;
            }

            if (!AttackPlayerLeft && !AttackPlayerRight) attackPlayer = false;
        }
    }

    private void WalkAndWaitToAttack()
    {
        if (!attackPlayer)
        {
            enemy.ContinueMoving();
            anim.SetBool("Move", true);
            RaycastHit2D detectLeft = Physics2D.Raycast(playerDetection.position, Vector2.left, distance);
            RaycastHit2D detectRight = Physics2D.Raycast(playerDetection.position, Vector2.right, distance);

            if (detectLeft.collider && detectLeft.collider.gameObject.tag == "Player")
            {
                attackPlayer = true;
                return;
            }

            if (detectRight.collider && detectRight.collider.gameObject.tag == "Player")
            {
                //Debug.Log("RIGHT ATTACK" + detectRight.collider);
                attackPlayer = true;
                return;
            }
        }

    }
}
