using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneTrigger : MonoBehaviour
{
    public string NameOfSceneToLoad;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(NameOfSceneToLoad);
        }
    }
}
