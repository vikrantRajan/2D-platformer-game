using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowProjectile;
    private GameObject myNewArrow;
    private bool firstArrow = false;
    private int arrowCount = 0;
    List<KeyValuePair<int, float>> arrowCreationList = new List<KeyValuePair<int, float>>();

    void addArrowToList(int i, float f)
    {
        arrowCreationList.Add(new KeyValuePair<int, float>(i, f));
    }


    public void CreateArrow()
    {
        
        arrowCount++;
        
        var index = arrowCount - 1;
        var prev = 0f;
        if(arrowCreationList.Count > 0)
        {
            prev = arrowCreationList[arrowCreationList.Count - 1].Value;
        }
            

        if (arrowCount > 3)
        {
            arrowCount = 1;
            index = 0;
            arrowCreationList.Clear();
        }

        addArrowToList(index, Time.time);
        var current = arrowCreationList[index].Value;
        if(current > prev + 0.5f && firstArrow)
        {
            myNewArrow = Instantiate(arrowProjectile, gameObject.transform.parent.gameObject.transform.position, Quaternion.identity) as GameObject;
            myNewArrow.transform.parent = gameObject.transform.parent.gameObject.transform;
        }

        if(!firstArrow)
        {
            myNewArrow = Instantiate(arrowProjectile, gameObject.transform.parent.gameObject.transform.position, Quaternion.identity) as GameObject;
            myNewArrow.transform.parent = gameObject.transform.parent.gameObject.transform;
            firstArrow = true;
        }

    }
}
