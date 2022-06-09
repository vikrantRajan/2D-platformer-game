using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruthpickHand : MonoBehaviour
{
    [Header("Hand Shakiness Intervals")]
    public float interval = 0.65f;
    private float i;

    [Header("Hand Shakiness Distance")]
    public float maxShakeDistance = 0.4f;
    public float minShakeDistance = 0.01f;

    [Header("Hand Movement Speed")]
    public float speed = 2f;

    [Header("Max Movement Distance")]
    public float top = -3.5f;
    public float bottom = -9.4f;
    public float left = -2f;
    public float right = 7.5f;

    [Header("Hand Sprites")]
    private SpriteRenderer spriteRenderer;
    public Sprite clenchedFistSprite;
    public Sprite openHandSprite;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        i = interval;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        HandleSpriteChange();
        HandShakiness();
    }

    private void HandShakiness()
    {
        if (Time.time > interval)
        {
            interval += i;

            float c = Mathf.Round(Random.Range(0f, 5f));
            if (c <= 1f) gameObject.transform.Translate(Vector3.left * Random.Range(minShakeDistance, maxShakeDistance));
            if (c > 1f && c <= 2f) gameObject.transform.Translate(Vector3.up * Random.Range(minShakeDistance, maxShakeDistance));
            if (c > 2f && c <= 3f) gameObject.transform.Translate(Vector3.right * Random.Range(minShakeDistance, maxShakeDistance));
            if (c > 3f && c <= 4f) gameObject.transform.Translate(Vector3.down * Random.Range(minShakeDistance, maxShakeDistance));

        }
    }

    private void HandleSpriteChange()
    {
        if (Input.GetMouseButton(0))
        {
            spriteRenderer.sprite = clenchedFistSprite;
            //Debug.Log("Clench Fist" + spriteRenderer.sprite);
        }
        else
        {
            spriteRenderer.sprite = openHandSprite;
            //Debug.Log("Open Hand" + spriteRenderer.sprite);
        }
    }

    private void Movement()
    {
        if (Input.GetKey(KeyCode.A))
        {

            if (gameObject.transform.position.x > left)
                gameObject.transform.Translate(Vector3.left * Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.D))
        {

            if (gameObject.transform.position.x < right)
                gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.W))
        {

            if (gameObject.transform.position.y < top)
                gameObject.transform.Translate(Vector3.up * Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.S))
        {

            if (gameObject.transform.position.y > bottom)
                gameObject.transform.Translate(Vector3.down * Time.deltaTime * speed);
        }

    }
}
