using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AttackZoneType type;
    public float projectileSpeed;

    [HideInInspector]
    public int damage = 5;
    [HideInInspector]
    public int mana_gain = 0;
    [HideInInspector]
    public Vector2 knockback;
    [HideInInspector]
    public Vector2 dir = Vector2.right;


    private Rigidbody2D theRB;


    // Start is called before the first frame update
    void Start()
    {

        theRB = GetComponent<Rigidbody2D>();

        float angle = Mathf.Atan2(dir.normalized.y, dir.normalized.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }

    // Update is called once per frame
    void Update()
    {

        theRB.velocity = projectileSpeed * dir;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (type == AttackZoneType.Player && other.GetComponent<Enemy>())
        {
            PlayerCharacter.Get().AddMana(mana_gain);
            other.GetComponent<Enemy>().TakeDamage(damage);
            if (knockback.magnitude > 0.1f)
            {
                Vector3 dir = other.transform.position - transform.position;
                Vector3 knock = new Vector3(Mathf.Sign(dir.x) * knockback.x, knockback.y); 
                other.GetComponent<Enemy>().PushBack(knock);
            }
        }

        if (type == AttackZoneType.Enemy && other.GetComponent<PlayerCharacter>())
        {
            other.GetComponent<PlayerCharacter>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }

}
