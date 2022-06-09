using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int attack_damage = 10;
    private PlayerCharacter playerCharacter;
    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            playerCharacter.TakeDamage(attack_damage);
            Destroy(gameObject);
        }
    }
}
