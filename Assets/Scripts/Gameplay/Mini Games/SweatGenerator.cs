using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatGenerator : MonoBehaviour
{
    [Header("Sweat Drop Prefab")]
    public GameObject sweatDropPrefab;

    [Header("Random Colors")]
    public Color colorOne;
    public Color colorTwo;
    public Color colorThree;
    public Color colorFour;
    public Color colorFive;

    [Header("Sweat Spawn Speed")]
    public float interval = 0.5f;
    private float i = 0f;

    [Header("Spawn Zone")]
    public float maxXaxis = 5f;
    public float minXaxis = -1f;
    public float maxYaxis = -1f;
    public float minYaxis = -2f;



    void Start()
    {
        i = interval;
    }

    void Update()
    {
        SpawnSweatDrop();

    }

    private void SpawnSweatDrop()
    {
        if (Time.time > interval)
        {
            interval += i;

            // creating new sweat drop, making sure its a child to the sweat generator.
            Vector2 pos = new Vector2(Random.Range(minXaxis, maxXaxis), Random.Range(minYaxis, maxYaxis));
            GameObject myNewSweatDrop = Instantiate(sweatDropPrefab, pos, Quaternion.identity) as GameObject;
            myNewSweatDrop.transform.parent = gameObject.transform;

            // Adding a random color to the new sweat drop
            float c = Mathf.Round(Random.Range(0f, 5f));
            if(c <= 1f) myNewSweatDrop.GetComponent<SpriteRenderer>().color = colorOne;
            if (c > 1f && c <= 2f) myNewSweatDrop.GetComponent<SpriteRenderer>().color = colorTwo;
            if (c > 2f && c <= 3f) myNewSweatDrop.GetComponent<SpriteRenderer>().color = colorThree;
            if (c > 3f && c <= 4f) myNewSweatDrop.GetComponent<SpriteRenderer>().color = colorFour;
            if (c > 4f && c <= 5f) myNewSweatDrop.GetComponent<SpriteRenderer>().color = colorFive;

        }
    }
}
