using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float move_speed = 4f;
    private float current_move_speed;
    public int attack_damage = 5;
    public int hp_max = 10;
    public float push_back = 5f;
    public float push_decel = 5f;

    [Header("Ground Detect")]
    public LayerMask ground_layer;
    public float ground_radius = 0.2f;
    public float death_y = -5f;

    [Header("References")]
    public AttackZone attack_zone;
    public GameObject hp_bar_prefab;
    public Transform base_root;

    private Rigidbody2D rigid;
    private SpriteRenderer render;
    private CapsuleCollider2D collide;
    private Animator animator;
    private ContactFilter2D ground_filter;

    private int hp;
    private bool is_dead = false;
    private float hit_timer = 0f;
    private bool isGrounded;
    private bool isNearCliff;
    private bool isFronted;
    private float move_side = -1f;
    private Vector3 move;
    private Vector3 facing;

    private float walk_timer = 0f;
    private Vector3 push_vector = Vector3.zero;

    private static List<Enemy> enemy_list = new List<Enemy>();

    void Awake()
    {
        enemy_list.Add(this);
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = GetComponentInChildren<SpriteRenderer>();
        collide = GetComponent<CapsuleCollider2D>();
        hp = hp_max;

        attack_zone.onHitPlayer += HitPlayer;

        ground_filter = new ContactFilter2D();
        ground_filter.layerMask = ground_layer;
        ground_filter.useLayerMask = true;
        ground_filter.useTriggers = false;

        current_move_speed = move_speed;
    }


    private void OnDestroy()
    {
        enemy_list.Remove(this);
    }

    private void Start()
    {
        if (hp_bar_prefab != null)
        {
            GameObject bar = Instantiate(hp_bar_prefab, transform.position, Quaternion.identity);
            bar.GetComponent<EnemyHPBar>().target = this;
        }
    }

    void Update()
    {
        if (TheGame.Get().IsPaused())
            return;

        if (is_dead)
            return;

        walk_timer += Time.deltaTime;

        isGrounded = DetectGrounded();
        isFronted = DetectFronted();
        isNearCliff = DetectNearCliff();

        //Change side
        if (isGrounded && walk_timer > 0.5f && (isNearCliff || isFronted))
        {
            move_side = -move_side;
            walk_timer = 0f;
            isNearCliff = false;
            isFronted = false;
        }

        Vector3 move_vect = Vector3.right * move_side * move_speed;
        float gravity = isGrounded ? 0f : rigid.gravityScale;
        if (push_vector.magnitude > 0.05f)
            rigid.velocity = new Vector2(push_vector.x, push_vector.y - gravity);
        else
            rigid.velocity = new Vector2(move_vect.x, rigid.velocity.y);

        //Push vector
        push_vector = Vector3.MoveTowards(push_vector, Vector3.zero, push_decel * Time.deltaTime);

        //Facing
        if (move_vect.magnitude > 0.1f)
            facing = new Vector3(move_vect.x, 0f).normalized;

        move = rigid.velocity;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(facing.x), transform.localScale.y, transform.localScale.z);

        if (!is_dead && transform.position.y < death_y)
            Kill();

        //Hit fx
        hit_timer += Time.deltaTime;
        if (hit_timer < 0f)
        {
            float alpha = Mathf.Abs(Mathf.Cos(hit_timer * 4f * Mathf.PI));
            render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);
        }
        else if (hit_timer < 1f)
        {
            render.color = new Color(render.color.r, render.color.g, render.color.b, 1f);
        }

        animator.SetBool("Move", move.magnitude > 0.1f);
    }

    public void PushBack(Vector3 dir)
    {
        push_vector += dir * push_back;
    }

    public void TakeDamage(int damage)
    {
        if (!is_dead)
        {
            if (hit_timer > 0f)
            {
                //hit_timer = -2f;
                this.hp -= damage;
                this.hp = Mathf.Clamp(this.hp, 0, hp_max);

                if (hp <= 0)
                    Kill();
                else
                    animator.SetTrigger("Hit");
            }
        }
    }

    public void Kill()
    {
        if (!is_dead)
        {
            hp = 0;
            is_dead = true;
            animator.SetTrigger("Death");
            Destroy(gameObject, 2f);
        }
    }

    public bool CanBeHit()
    {
        return hit_timer > 0f;
    }

    private void HitPlayer(PlayerCharacter player)
    {
        if (!is_dead)
        {
            player.TakeDamage(attack_damage);
        }
    }

    private bool DetectGrounded()
    {
        Vector3 pos = base_root.transform.position;
        float radius = ground_radius;
        //Debug.DrawRay(pos, Vector3.down * radius, Color.white);
        return Physics2D.OverlapCircle(pos, radius, ground_layer.value);
    }

    private bool DetectNearCliff()
    {
        float front_dist = GetCapsuleRadius() + ground_radius * 2f;
        Vector3 side = Vector3.right * move_side * front_dist;
        Vector3 pos = GetCapsulePos(Vector3.down) + side;
        Vector3 dir = Vector3.down * (GetCapsuleRadius() + ground_radius * 2f);
        Debug.DrawRay(pos, dir, Color.yellow);

        bool hit = RaycastObstacle(pos, dir, ground_filter);
        return !hit;
    }

    private bool DetectFronted()
    {
        float front_dist = GetCapsuleRadius() + ground_radius * 2f;
        Vector3 side_vect = Vector3.right * move_side * front_dist;
        Debug.DrawRay(GetCapsulePos(Vector2.zero), side_vect);
        return RaycastObstacle(GetCapsulePos(Vector3.up), side_vect, ground_filter)
               || RaycastObstacle(GetCapsulePos(Vector3.down), side_vect, ground_filter)
               || RaycastObstacle(GetCapsulePos(Vector2.zero), side_vect, ground_filter);
    }

    private bool RaycastObstacle(Vector2 pos, Vector2 dir, ContactFilter2D filter)
    {
        RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
        //Debug.DrawRay(pos, dir);
        Physics2D.Raycast(pos, dir.normalized, filter, hitBuffer, dir.magnitude);
        for (int j = 0; j < hitBuffer.Length; j++)
        {
            if (hitBuffer[j].collider != null && hitBuffer[j].collider != collide && !hitBuffer[j].collider.isTrigger)
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetCapsulePos(Vector3 dir)
    {
        Vector2 orientation = dir.normalized;
        Vector2 raycast_offset = new Vector2(collide.offset.x * GetFacingSide(), collide.offset.y) + orientation * Mathf.Abs(collide.size.y * 0.5f - collide.size.x * 0.5f);
        return rigid.position + raycast_offset * collide.transform.lossyScale.y;
    }

    private float GetCapsuleRadius()
    {
        return GetSize().x * 0.5f;
    }

    public Vector2 GetSize()
    {
        return new Vector2(Mathf.Abs(collide.transform.lossyScale.x) * collide.size.x, Mathf.Abs(collide.transform.lossyScale.y) * collide.size.y);
    }

    public float GetFacingSide()
    {
        return Mathf.Sign(facing.x);
    }

    public void StopMoving()
    {
        move_speed = 0f;
    }

    public void ContinueMoving()
    {
        move_speed = current_move_speed;
    }

    public int GetHP()
    {
        return hp;
    }

    public bool IsDead()
    {
        return is_dead;
    }

    public static List<Enemy> GetAll()
    {
        return enemy_list;
    }
}
