using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<SpriteRenderer>())
            GetComponent<SpriteRenderer>().enabled = false;
    }
}
