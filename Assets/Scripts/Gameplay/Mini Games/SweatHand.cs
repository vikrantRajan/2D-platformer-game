using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatHand : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        HandleSpriteChange();
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
