using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    public float spawnRate = 2f;
    public float movingSpeed = 2f;
    public float time_until_destroyed = 5f;
    public GameObject movingHazardPrefab;
    private GameObject myNewHazard;
    private PlayerCharacter playerCharacter;
    private float i;
    
    public enum ShootDirection
    {
        Left,
        LeftTop,
        Top,
        RightTop,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
    }
    public ShootDirection shootingDirection;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(shootingDirection);
        i = spawnRate;
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawnRate)
        {
            spawnRate += i;
            SpawnHazard();
        }

        if (playerCharacter.IsDead()) Destroy(gameObject);

    }

    public void SpawnHazard()
    {
        myNewHazard = Instantiate(movingHazardPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        myNewHazard.transform.parent = gameObject.transform;
        Destroy(myNewHazard, time_until_destroyed);

    }
}
