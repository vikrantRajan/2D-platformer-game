using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatCollisions : MonoBehaviour
{
    private float sweatHealth = 10f;
    private float damage = 5f;
    private float t = 0;

    private void Update()
    {
        UpdatePoints();
        DamageUpdate();
    }

    private void DamageUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            damage = 10f;
            //Debug.Log("Clench Fist" + damage);
        }
        else
        {
            damage = 5f;
            //Debug.Log("Open Hand" + damage);
        }
    }

    private void UpdatePoints()
    {
        if (sweatHealth <= 0f)
        {
            Destroy(gameObject);
            SweatMiniGame.Get().AddPoint();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SweatHand>())
        {
           // Debug.Log("hit sweat drop");
            sweatHealth -= damage;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<SweatHand>())
        {
            
            t += Time.deltaTime;

            if (Input.GetMouseButton(0))
            {
                if (t >= 0.1)
                {
                    sweatHealth = 0;
                }
            } else
            {
                if (t >= 0.3)
                {
                    sweatHealth = 0;
                }
            }

        }
    }


}
