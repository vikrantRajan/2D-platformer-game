using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingHazard : MonoBehaviour
{
    public int attack_damage = 10;
    private PlayerCharacter playerCharacter;
    private HazardSpawner hazardSpawner;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
        hazardSpawner = gameObject.transform.parent.gameObject.GetComponent<HazardSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCharacter.IsDead()) Destroy(gameObject);
        else Move();
    }

    private void Move()
    {
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.Left)
        {
            gameObject.transform.Translate(Vector3.left * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.LeftTop)
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * hazardSpawner.movingSpeed);
            gameObject.transform.Translate(Vector3.left * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.Top)
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.RightTop)
        {
            gameObject.transform.Translate(Vector3.right * Time.deltaTime * hazardSpawner.movingSpeed);
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.Right)
        {
            gameObject.transform.Translate(Vector3.right * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.BottomRight)
        {
            gameObject.transform.Translate(Vector3.right * Time.deltaTime * hazardSpawner.movingSpeed);
            gameObject.transform.Translate(Vector3.down * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.Bottom)
        {
            gameObject.transform.Translate(Vector3.down * Time.deltaTime * hazardSpawner.movingSpeed);
        }
        if (hazardSpawner.shootingDirection == HazardSpawner.ShootDirection.BottomLeft)
        {
            gameObject.transform.Translate(Vector3.left * Time.deltaTime * hazardSpawner.movingSpeed);
            gameObject.transform.Translate(Vector3.down * Time.deltaTime * hazardSpawner.movingSpeed);
        }
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
