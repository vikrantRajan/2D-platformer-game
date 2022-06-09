using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    public Enemy target;
    public Vector3 offset = Vector3.up;
    public ProgressBar bar;

    void Start()
    {
        if(target != null)
            transform.position = target.transform.position + offset;

    }

    void Update()
    {
        if (target == null || target.IsDead())
        {
            Destroy(gameObject);
            return;
        }

        transform.position = target.transform.position + offset;
        bar.SetValue(target.GetHP());
        bar.SetMax(target.hp_max);
    }
}
