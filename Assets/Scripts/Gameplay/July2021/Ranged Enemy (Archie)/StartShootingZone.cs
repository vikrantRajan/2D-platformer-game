using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartShootingZone : MonoBehaviour
{
    private RangedEnemy rangedEnemy;
    // Start is called before the first frame update
    void Start()
    {
        rangedEnemy = gameObject.transform.parent.gameObject.GetComponent<RangedEnemy>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rangedEnemy.UpdateAttack(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rangedEnemy.UpdateAttack(false);
        }
    }
}
