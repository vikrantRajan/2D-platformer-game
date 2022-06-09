using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickZone : MonoBehaviour
{
    private bool pickedUpToothPick = false;
    [Header("Add the parent prefab here")]
    public GameObject TruthpickMiniGameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pickedUpToothPick && !Input.GetMouseButton(0))
        {
            TruthpickMiniGame.Get().AddPoint();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetMouseButton(0))
        {
            if (collision.gameObject.name == "Truthpick" || collision.gameObject.name == "Truthpick(Clone)")
            {
                collision.gameObject.transform.SetParent(gameObject.transform.parent);
                pickedUpToothPick = true;
                
            }
        }
        else if (!Input.GetMouseButton(0) && (collision.gameObject.name == "Truthpick" || collision.gameObject.name == "Truthpick(Clone)"))
                collision.gameObject.transform.SetParent(TruthpickMiniGameObject.transform);
    }
}
