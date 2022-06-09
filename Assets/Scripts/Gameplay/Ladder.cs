using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public bool is_rope = false;

    public bool is_swing = false;
    public Transform top_ref;
    public Transform bottom_ref;

    private Collider2D collide;

    private static List<Ladder> ladder_list = new List<Ladder>();

    void Awake()
    {
        ladder_list.Add(this);
        collide = GetComponent<Collider2D>();
    }

    void OnDestroy()
    {
        ladder_list.Remove(this);
    }

    void Update()
    {

    }

    public float GetPercentAt(Vector3 pos)
    {
        if (top_ref != null && bottom_ref != null)
        {
            Vector3 up_total = (top_ref.position - bottom_ref.position);
            Vector3 up = up_total.normalized;
            Vector3 pdir = pos - bottom_ref.position;
            Vector3 p1 = Vector3.Project(pdir, up) + bottom_ref.position;

            Vector3 d1 = (p1 - bottom_ref.position);
            Vector3 d2 = (top_ref.position - p1);

            if (d1.magnitude > up_total.magnitude)
                return 1f;
            else if (d2.magnitude > up_total.magnitude)
                return 0f;
            else
                return d1.magnitude / up_total.magnitude;
        }
        return 0f;
    }

    public Vector3 GetPositionAt(float percent)
    {
        if (top_ref != null && bottom_ref != null)
        {
            return top_ref.position * percent + bottom_ref.position * (1f - percent);
        }
        return transform.position;
    }

    public Vector3 GetUpVect()
    {
        if (top_ref != null && bottom_ref != null)
            return (top_ref.position - bottom_ref.position).normalized;
        return Vector3.up;
    }

    public static Ladder GetOverlapLadder(Vector3 pos)
    {
        foreach (Ladder ladder in ladder_list)
        {
            if (ladder.collide.OverlapPoint(pos))
                return ladder;
        }
        return null;
    }

    public static List<Ladder> GetAll()
    {
        return ladder_list;
    }
}