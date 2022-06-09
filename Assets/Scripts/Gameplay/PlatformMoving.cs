using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformMovingType
{
    Static = 0,
    Move = 5,
    Rotate=7,
    Fall = 10,
    Destroy = 15,
}

public class PlatformMoving : MonoBehaviour
{
    public PlatformMovingType type;
    public Vector2 move_vect;
    public float move_speed = 2f;
    public float pause_duration = 0f;

    [Header("Rotate Platform")]
    public float rotate_angle = 0f;
    public float drag_factor = 1f;

    [Header("Fall Platform")]
    public float fall_delay = 1f;
    public float respawn_delay = 10f;
    public string destroy_anim = "Destroy";

    private Animator animator;
    private Rigidbody2D rigid;
    private Collider2D collide;
    private Vector3 start_pos;
    private Vector3 move = Vector3.zero;
    private float timer = 0f;
    private float angle = 0f;
    private bool touched = false;
    private bool move_side = false;
    private bool paused = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        collide = GetComponent<Collider2D>();
        start_pos = rigid.position;
    }

    void FixedUpdate()
    {
        if (TheGame.Get().IsPaused())
            return;

        if (paused)
        {
            timer += Time.fixedDeltaTime;
            if(timer > pause_duration)
            {
                timer = 0f;
                paused = false;
            }
            return;
        }

        if (type == PlatformMovingType.Move)
        {
            Vector2 target = move_side ? start_pos : start_pos + new Vector3(move_vect.x, move_vect.y, 0f);
            Vector2 dir = target - rigid.position;
            float dir_move = Mathf.Min(dir.magnitude, move_speed * Time.fixedDeltaTime);
            move = dir.normalized * move_speed;
            rigid.position += dir.normalized * dir_move;

            if (dir.magnitude < 0.1f)
            {
                move_side = !move_side;
                move = Vector3.zero;
                paused = pause_duration > 0.01f;
                timer = 0f;
            }
        }

        if (type == PlatformMovingType.Rotate)
        {
            angle += move_speed * Mathf.PI * 0.25f * (move_side ? -1f : 1f) * Time.fixedDeltaTime; //Radian angle
            float angle_deg = angle * Mathf.Rad2Deg;
            float current_side = Mathf.Sign(move_speed) * (move_side ? -1f : 1f);
            Vector3 center = start_pos + new Vector3(move_vect.x, move_vect.y, 0f);
            float radius = move_vect.magnitude;

            Vector2 prev_move = rigid.position;
            float oangle = angle - Mathf.PI * 0.5f;
            rigid.position = center + new Vector3(Mathf.Cos(oangle), Mathf.Sin(oangle), 0f) * radius;
            Vector2 dir = rigid.position - prev_move;
            move = dir.normalized * move_speed * drag_factor * move_vect.magnitude;

            if (rotate_angle < 180f) {
                if ((angle_deg > rotate_angle && current_side > 0f) || (angle_deg < -rotate_angle && current_side < 0f))
                {
                    move_side = !move_side;
                    move = Vector3.zero;
                    paused = pause_duration > 0.01f;
                    timer = 0f;
                }
            }
        }

        if (type == PlatformMovingType.Fall)
        {
            if (touched)
            {
                timer += Time.fixedDeltaTime;
                if (timer > fall_delay)
                {
                    move = Vector3.down * move_speed;
                    rigid.position += Vector2.down * move_speed * Time.fixedDeltaTime;
                }
                else
                {
                    rigid.position = start_pos + new Vector3(Mathf.Cos(timer * 8f * Mathf.PI) * 0.05f, 0f, 0f);
                }

                if (timer > respawn_delay)
                {
                    ResetPlatform();
                }
            }
        }

        if (type == PlatformMovingType.Destroy)
        {
            if (touched)
            {
                timer += Time.fixedDeltaTime;
                if (timer > fall_delay)
                {
                    collide.enabled = false;
                    touched = false;
                }

                if (timer > respawn_delay)
                {
                    ResetPlatform();
                }
            }
        }
    }

    public void ResetPlatform()
    {
        timer = 0f;
        rigid.position = start_pos;
        collide.enabled = true;
        touched = false;
        move = Vector3.zero;

        if (animator)
            animator.Rebind();
    }

    public void OnCharacterStep()
    {
        if (type == PlatformMovingType.Fall || type == PlatformMovingType.Destroy)
        {
            if (!touched)
            {
                touched = true;
                timer = 0f;

                if (type == PlatformMovingType.Destroy && animator)
                    animator.SetTrigger(destroy_anim);
            }
        }
    }

    public Vector3 GetMove()
    {
        return move;
    }

    public Vector2 GetMove2()
    {
        return new Vector2(move.x, move.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 dir = collision.gameObject.transform.position - transform.position;
            if (dir.y > 0f)
            {
                OnCharacterStep();
            }
        }
    }
}
