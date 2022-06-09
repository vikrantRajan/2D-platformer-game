using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public float shootingIntervals = 2f;
    public float arrowSpeed = 2f;
    public float destroy_arrow_timer = 5f;
    public GameObject startShootingZone;
    public ArrowGenerator arrowGenerator;

    private bool arrowMovingLeft = true;
    private Enemy enemy;
    private Animator anim;
    private bool attackPlayer = false;
    private float i = 0f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        i = shootingIntervals;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1, 1, 1);
        enemy.StopMoving();
        if (!enemy.IsDead())
        {
            WaitToAttack();
            AttackPlayer();
        }
        if (enemy.IsDead())
        {
            anim.SetTrigger("Death");
        }

    }

    private void AttackPlayer()
    {
        if (attackPlayer)
        {
            anim.SetTrigger("Attack");
            if (Time.time > shootingIntervals)
            {
                shootingIntervals += i;
                arrowGenerator.CreateArrow();

                ShootingDirection();

            }
        }
    }

    private void ShootingDirection()
    {
        if (gameObject.transform.rotation.y == 1.0) arrowMovingLeft = false;
        else arrowMovingLeft = true;

    }

    public bool GetShootDirection()
    {
        return arrowMovingLeft;
    }

    public void UpdateAttack(bool x)
    {
        attackPlayer = x;
    }

    private void WaitToAttack()
    {
        if (!attackPlayer)
        {

            anim.SetTrigger("Idle");
            ShootingDirection();

        }

    }

}
