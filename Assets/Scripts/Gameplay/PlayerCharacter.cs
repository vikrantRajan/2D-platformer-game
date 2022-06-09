using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerCharacterState
{
    Normal = 0,
    Climb = 10,
    ClimbSlide = 15,
    LedgeGrab = 20,
    LedgeGrabUp = 22,
    Dead = 50,
}

[System.Serializable]
public class PlayerAttack
{
    public string id;
    public int damage = 1;
    public int mana = 0;
    public float attack_windup = 0.5f;
    public float attack_windout = 0.5f;
    public AttackZone attack_zone;
    public GameObject projectile;
    public bool invulnerable;
    public bool aim_downward;
    public Vector2 dash;
    public Vector2 knockback;
    public string anim;

}

public class PlayerCharacter : MonoBehaviour
{
    public int player_id;

    [Header("Movement")]
    public float move_speed = 5f;

    [Header("Jump")]
    public float jump_force = 15f;
    public float jump_hold_gravity_ratio = 0.5f;
    public float jump_duration_min = 0.2f;
    public float jump_duration_max = 0.5f;
    public float coyote_duration = 0.25f;

    [Header("Crouch")]
    public float crouch_speed_factor = 0.5f;
    public float crouch_collider_factor = 0.5f;

    [Header("Ledge Grab")]
    public float grab_dist_x = 0.3f;
    public float grab_dist_up = 0.3f;
    public float grab_dist_down = 0.1f;
    public float grab_climb_duration = 1f;
    public float grab_climb_distance = 0.3f;
    public float grab_cooldown = 0.8f;

    [Header("Ladder")]
    public float slide_speed = 10f;
    public float slide_decel = 10f;

    [Header("Ground Detect")]
    public LayerMask ground_layer;
    public LayerMask ground_layer_semi;
    public float ground_radius = 0.2f;
    public float death_y = -5f;

    [Header("Stats")]
    public int hp_max = 100;
    public int mana_max = 100;

    [Header("Attacks")]
    public PlayerAttack[] attacks;

    [Header("References")]
    public GameObject projectile_prefab;
    public Transform base_root;
    public Transform feet_root;
    public Transform shoot_root;
    public Transform shoot_root_aerial;
    public Transform ledge_grab_root;
    public BoxCollider2D semi_collider;

    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer render;
    private BoxCollider2D collide;
    private PlayerControls controls;
    private ContactFilter2D ground_filter;
    private ContactFilter2D ground_filter_semi;
    private ContactFilter2D ground_filter_both;

    private PlayerCharacterState state = PlayerCharacterState.Normal;
    private Vector3 start_pos;
    private float start_gravity;
    private float start_collide_height;
    private float start_collide_y;
    private Vector3 last_ground_pos;
    private Vector3 grab_pos;
    private bool isJumping = false;
    private bool wasGrounded = false;
    private bool wasGroundedSemi = false;
    private bool isGrounded = false;
    private bool isGroundedSolid = false;
    private bool isGroundedSemi = false;
    private bool isTopped = false;
    private bool isFronted = false;
    private bool isFrontedSemi = false;
    private bool isCrouch = false;
    private Ladder snap_to_ladder = null;
    private bool climb_rope = false;
    private bool go_through_semi = false;
    private bool overlap_obstacle = false;
    private bool is_moving_platform = false;
    private float platform_timer = 0f;
    private PlatformMoving prev_platform = null;

    private float state_timer = 0f;
    private float jump_timer = 0f;
    private float air_timer = 0f;
    private float hit_timer = 0f;

    private int hp = 100;
    private int mana = 100;

    private Vector3 move;
    private Vector3 facing;
    private bool is_attacking = false;
    private bool is_doing_action = false; //Cant move when true
    private bool is_protected = false;
    private Vector3 attack_dash = Vector3.zero;

    private HashSet<Enemy> hit_enemy_list = new HashSet<Enemy>();

    private static PlayerCharacter _instance;

    void Awake()
    {
        _instance = this;
        rigid = GetComponent<Rigidbody2D>();
        collide = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
        render = GetComponentInChildren<SpriteRenderer>();
        controls = GetComponent<PlayerControls>();
        start_pos = transform.position;
        start_gravity = rigid.gravityScale;
        last_ground_pos = transform.position;
        start_collide_height = collide.size.y;
        start_collide_y = collide.offset.y;
        hp = hp_max;
        mana = mana_max;

        ground_filter = new ContactFilter2D();
        ground_filter.layerMask = ground_layer;
        ground_filter.useLayerMask = true;
        ground_filter.useTriggers = false;

        ground_filter_semi = new ContactFilter2D();
        ground_filter_semi.layerMask = ground_layer_semi;
        ground_filter_semi.useLayerMask = true;
        ground_filter_semi.useTriggers = false;

        ground_filter_both = new ContactFilter2D();
        ground_filter_both.layerMask = ground_layer | ground_layer_semi;
        ground_filter_both.useLayerMask = true;
        ground_filter_both.useTriggers = false;

    }

    private void Start()
    {
        foreach (PlayerAttack attack in attacks)
        {
            if (attack.attack_zone != null)
            {
                PlayerAttack cattack = attack;
                attack.attack_zone.onHitEnemy += (Enemy enemy) => { OnHitEnemy(cattack, enemy); };
                attack.attack_zone.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (TheGame.Get().IsPaused())
            return;

        state_timer += Time.deltaTime;
        jump_timer += Time.deltaTime;

        isFronted = DetectFronted(ground_filter);
        isFrontedSemi = DetectFrontedSemi(ground_filter_semi);
        wasGrounded = isGrounded || air_timer < coyote_duration;
        wasGroundedSemi = isGroundedSemi || air_timer < coyote_duration;
        isGroundedSemi = !isFrontedSemi && DetectGrounded(ground_layer_semi.value);
        isGroundedSolid = DetectGrounded(ground_layer.value);
        overlap_obstacle = DetectOverlapObstacle();
        isGrounded = isGroundedSemi || isGroundedSolid;
        isTopped = DetectTopped();

        bool shouldCrouch = controls.GetMove().y < -0.1f || (isTopped && isGrounded);
        bool canCrouch = (isGrounded && state == PlayerCharacterState.Normal) || state == PlayerCharacterState.LedgeGrabUp;
        isCrouch = shouldCrouch && canCrouch;
        
        air_timer += Time.deltaTime;
        if (isGrounded && !isJumping)
            air_timer = 0;

        rigid.velocity = new Vector2(0f, rigid.velocity.y);

        if (state == PlayerCharacterState.Normal)
        {
            float speed = isCrouch ? move_speed * crouch_speed_factor : move_speed;
            if (!is_doing_action)
            {
                if (controls.GetMove().x < -0.1f)
                    rigid.velocity = new Vector2(-speed, rigid.velocity.y);
                else if (controls.GetMove().x > 0.1f)
                    rigid.velocity = new Vector2(speed, rigid.velocity.y);
            }

            if(is_attacking)
            {
                rigid.velocity += new Vector2(attack_dash.x, 0f);
            }

            UpdateFacing();

            //Must be after Update facing
            if (isFronted)
                rigid.velocity = new Vector2(0f, rigid.velocity.y);

            if (!is_attacking && controls.IsPressJump() && wasGrounded && !isCrouch)
                Jump();
            
            Ladder ladder = Ladder.GetOverlapLadder(transform.position);
            if (!is_attacking && ladder && controls.GetMove().y > 0.1f && state_timer > 0.1f && !isJumping)
                Climb(ladder);

            if (!is_attacking && controls.GetMove().y > 0.1f)
                CheckLedgeGrab();
        }

        if (state == PlayerCharacterState.Climb)
        {
            Ladder ladder = Ladder.GetOverlapLadder(transform.position);
            if (snap_to_ladder)
                ladder = snap_to_ladder;

            if (ladder == null)
            {
                StopClimb();
                return;
            }

            if (isGrounded && controls.GetMove().y < -0.1f)
            {
                StopClimb();
                return;
            }

            Vector3 upvect = ladder.GetUpVect();
            rigid.velocity = new Vector2(0f, 0f);

            if (controls.GetMove().y < -0.1f)
                rigid.velocity = upvect * -move_speed;
            else if (controls.GetMove().y > 0.1f)
                rigid.velocity = upvect * move_speed;

            UpdateFacing();

            Ladder ladderTop = Ladder.GetOverlapLadder(transform.position + upvect * 0.5f);
            bool top = ladderTop == null && ladder != null;
            if (top && controls.GetMove().y > 0.1f)
                rigid.velocity = Vector2.zero;

            if (snap_to_ladder)
            {
                float percent = snap_to_ladder.GetPercentAt(transform.position);
                transform.position = snap_to_ladder.GetPositionAt(percent) + (Vector3)rigid.velocity * 0.5f * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(upvect.y, upvect.x) * Mathf.Rad2Deg - 90f);
            }

            if (controls.IsPressJump() && snap_to_ladder == null && controls.GetMove().y < -0.1f)
                SlideDown();
            else if (controls.IsPressJump())
                Jump();
        }

        if (state == PlayerCharacterState.ClimbSlide)
        {
            Vector2 target = new Vector2(0f, 0f);

            if (!controls.IsHoldJump())
                rigid.velocity = Vector2.MoveTowards(rigid.velocity, target, slide_decel * Time.deltaTime);

            Ladder ladder = Ladder.GetOverlapLadder(transform.position);

            if (ladder != null && rigid.velocity.y > -0.1f)
            {
                state = PlayerCharacterState.Climb;
                state_timer = 0f;
                rigid.velocity = Vector2.zero;
            }

            if (ladder == null || isGrounded)
            {
                StopClimb();
                rigid.velocity = Vector2.zero;
            }
        }

        if (state == PlayerCharacterState.LedgeGrab)
        {
            rigid.velocity = new Vector2(0f, 0f);
            UpdateLedgeGrab();
        }

        if (state == PlayerCharacterState.LedgeGrabUp)
        {
            UpdateLedgeGrabUp();
        }

        if (isJumping)
        {
            if (jump_timer > jump_duration_max)
                isJumping = false;

            if (jump_timer > jump_duration_min && !controls.IsHoldJump())
                isJumping = false;
        }

        //bool stick_to_floor = (!wasGrounded && isGrounded) || (isGroundedSemi && !isJumping);
        rigid.gravityScale = (state == PlayerCharacterState.Climb || state == PlayerCharacterState.ClimbSlide
            || state == PlayerCharacterState.LedgeGrab ||
            state == PlayerCharacterState.LedgeGrabUp) ? 0f : start_gravity;

        if (isJumping && state == PlayerCharacterState.Normal)
            rigid.gravityScale = start_gravity * jump_hold_gravity_ratio;

        go_through_semi = (rigid.velocity.y > 0.01f || isFrontedSemi) && state == PlayerCharacterState.Normal && (!isGroundedSemi || isJumping);
        go_through_semi = go_through_semi || (overlap_obstacle && !isGroundedSemi);
        semi_collider.enabled = !go_through_semi;

        //int semi_layer = (int)Mathf.Log(ground_layer_semi.value, 2);
        //Physics2D.IgnoreLayerCollision(semi_layer, gameObject.layer, go_through_semi);
        //Debug.Log(overlap_obstacle);

        //if (stick_to_floor)
        //    rigid.velocity = new Vector2(rigid.velocity.x, 0f); //Stop jump

        move = rigid.velocity;

        if (state == PlayerCharacterState.Dead)
            return;

        if (!is_attacking && controls.IsPressMelee())
        {
            if (state == PlayerCharacterState.Normal)
            {
                if(!isGrounded)
                    TryAttack("melee_aerial");
                else if (isCrouch)
                    TryAttack("melee_crouch");
                else if (move.magnitude > 0.1f)
                    TryAttack("melee_dash");
                else
                    TryAttack("melee_stand");
            }
        }

        if (!is_attacking && controls.IsPressShoot())
        {
            if (state == PlayerCharacterState.Normal)
            {
                if (!isGrounded)
                    TryAttack("magic_aerial");
                else if (isCrouch)
                    TryAttack("magic_crouch");
                else if (move.magnitude > 0.1f)
                    TryAttack("magic_dash");
                else
                    TryAttack("magic_shoot");
            }
        }

        if (state != PlayerCharacterState.Dead && transform.position.y < death_y)
            Kill();

        collide.size = new Vector2(collide.size.x, start_collide_height * (isCrouch ? crouch_collider_factor : 1f));
        collide.offset = new Vector2(collide.offset.x, start_collide_y - (isCrouch ? start_collide_height * (1f - crouch_collider_factor) * 0.5f : 0f));

        anim.SetFloat("Speed", Mathf.Abs(rigid.velocity.x));
        anim.SetFloat("SpeedY", Mathf.Abs(rigid.velocity.y));
        anim.SetBool("Grounded", isGroundedSolid || (isGroundedSemi && !go_through_semi));
        anim.SetBool("Climb", !climb_rope && (state == PlayerCharacterState.Climb || state == PlayerCharacterState.ClimbSlide));
        anim.SetBool("ClimbRope", climb_rope && (state == PlayerCharacterState.Climb || state == PlayerCharacterState.ClimbSlide));
        anim.SetFloat("ClimbSide", Mathf.Sign(move.y));
        anim.SetBool("Crouch", isCrouch);
        anim.SetBool("LedgeHold", state == PlayerCharacterState.LedgeGrab);

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

    }

    private void FixedUpdate()
    {
        if(state == PlayerCharacterState.Normal)
        {

            CheckForPlatform();
        }
    }

    private void UpdateFacing()
    {
        if (rigid.velocity.x < 0)
        {
            facing = Vector3.left;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rigid.velocity.x > 0)
        {
            facing = Vector3.right;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void Jump()
    {
        state = PlayerCharacterState.Normal;
        rigid.velocity = new Vector2(rigid.velocity.x, jump_force);
        transform.rotation = Quaternion.identity;
        isJumping = true;
        collide.isTrigger = false;
        jump_timer = 0f;
        air_timer = coyote_duration;
        snap_to_ladder = null;
        state_timer = 0f;
        prev_platform = null;
        is_moving_platform = false;

        anim.SetTrigger("Jump");
    }

    public void SlideDown()
    {
        state = PlayerCharacterState.ClimbSlide;
        state_timer = 0f;
        rigid.velocity = new Vector2(rigid.velocity.x, -slide_speed);
        isJumping = false;
        jump_timer = 0f;
        anim.SetTrigger("ClimbSlide");
    }

    public void Climb(Ladder ladder)
    {
        transform.position = new Vector3(ladder.transform.position.x, transform.position.y, 0f);
        state = PlayerCharacterState.Climb;
        state_timer = 0f;
        isJumping = false;
        climb_rope = ladder.is_rope;
        snap_to_ladder = null;
        prev_platform = null;
        is_moving_platform = false;

        if (ladder.is_swing)
            snap_to_ladder = ladder;
    }

    public void StopClimb()
    {
        state = PlayerCharacterState.Normal;
        state_timer = 0f;
        snap_to_ladder = null;
        transform.rotation = Quaternion.identity;
        climb_rope = false;
    }

    public void TryAttack(string attack_id) 
    {
        if (!is_attacking && state == PlayerCharacterState.Normal)
        {
            PlayerAttack attack = GetAttack(attack_id);
            if (attack != null)
            {
                if (attack.mana <= 0 || mana >= attack.mana)
                    Attack(attack_id);
            }
        }
    }

    public void Attack(string attack_id)
    {
        PlayerAttack attack = GetAttack(attack_id);
        if (attack != null)
        {
            hit_enemy_list.Clear();
            if (attack.mana > 0)
                AddMana(-attack.mana);
            StartCoroutine(AttackRun(attack, !isGrounded));
        }
    }

    private IEnumerator AttackRun(PlayerAttack attack, bool aerial) {

        is_attacking = true;
        is_doing_action = !aerial;
        anim.SetTrigger(attack.anim);
        attack_dash = attack.dash;
        attack_dash = new Vector3(Mathf.Sign(transform.localScale.x) * attack_dash.x, attack_dash.y, 0f);
        is_protected = attack.invulnerable;

        yield return new WaitForSeconds(attack.attack_windup);

        if (attack.projectile != null)
        {
            Transform root = attack.aim_downward ? shoot_root_aerial : shoot_root;
            GameObject projectileClone = (GameObject)Instantiate(attack.projectile, root.position, root.rotation);
            Projectile proj = projectileClone.GetComponent<Projectile>();
            proj.dir = Vector3.right * Mathf.Sign(transform.localScale.x);
            proj.damage = attack.damage;
            proj.knockback = attack.knockback;
            if (attack.mana < 0)
                proj.mana_gain = -attack.mana;
            if (attack.aim_downward)
            {
                proj.dir += Vector2.down;
                proj.dir.Normalize();
            }
        }

        if (attack.attack_zone != null)
            attack.attack_zone.SetActive(true);

        yield return new WaitForSeconds(attack.attack_windout);

        if (attack.attack_zone != null)
            attack.attack_zone.SetActive(false);

        is_attacking = false;
        is_doing_action = false;
        is_protected = false;
    }

    public void AddMana(int mana)
    {
        this.mana += mana;
        this.mana = Mathf.Clamp(this.mana, 0, mana_max);
    }

    public void TakeDamage(int damage)
    {
        if (state != PlayerCharacterState.Dead)
        {
            if (hit_timer > 0f && !is_protected)
            {
                hit_timer = -2f;
                this.hp -= damage;
                this.hp = Mathf.Clamp(this.hp, 0, hp_max);

                if (hp <= 0)
                    Kill();
            }
        }
    }

    public void Kill()
    {
        if (state != PlayerCharacterState.Dead)
        {
            hp = 0;
            mana = 0;
            state = PlayerCharacterState.Dead;
            gameObject.SetActive(false);
            TheCamera.Get().Shake();
        }
    }

    public void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
        last_ground_pos = pos;
    }

    public void SetRespawnPoint(Vector3 pos)
    {
        last_ground_pos = pos;
    }

    private void CheckForPlatform()
    {
        is_moving_platform = false;

        bool platform_solid = isGroundedSolid && !isJumping;
        bool platform_semi = isGroundedSemi && !go_through_semi && !isJumping;

        if (platform_solid || platform_semi)
        {
            //Platform
            GameObject platform = DetectGroundedTag(ground_filter_both, "Platform");
            if (platform && platform.GetComponent<PlatformMoving>())
            {
                PlatformMoving pmoving = platform.GetComponent<PlatformMoving>();
                //transform.position += pmoving.GetMove() * Time.deltaTime;
                rigid.position += pmoving.GetMove2() * Time.fixedDeltaTime;   
                pmoving.OnCharacterStep();
                is_moving_platform = true;
                prev_platform = pmoving;
                platform_timer = 0f;
            }

            //Floor trigger
            /*GameObject floor_trigger = RaycastObstacleTag(center, Vector3.down * radius, contact_filter, "FloorTrigger");
            if (floor_trigger && floor_trigger.GetComponent<FloorTrigger>())
            {
                FloorTrigger ftrigger = floor_trigger.GetComponent<FloorTrigger>();
                ftrigger.Activate();
            }*/
        }

        if(prev_platform != null && !is_moving_platform)
        {
            platform_timer += Time.fixedDeltaTime;
            rigid.position += prev_platform.GetMove2() * Time.fixedDeltaTime;
            is_moving_platform = true;
            if (platform_timer > 0.05f)
                prev_platform = null;
        }
    }

    private void CheckLedgeGrab()
    {
        Vector3 hand_pos = ledge_grab_root.transform.position;
        Vector3 pos_high = hand_pos + Vector3.up * grab_dist_up;
        Vector3 pos_low = hand_pos + Vector3.down * grab_dist_down;
        Vector3 dir = GetFacing();
        float dirX, dirY;
        bool ray_high = RaycastObstacle(pos_high, dir * grab_dist_x * 2f, ground_filter);
        bool ray_low = RaycastObstacle(pos_low, dir * grab_dist_x, ground_filter, out dirX);

        if (!ray_high && ray_low && state_timer > grab_cooldown)
        {
            state = PlayerCharacterState.LedgeGrab;
            rigid.velocity = Vector2.zero;
            move = Vector3.zero;
            state_timer = 0f;
            collide.isTrigger = true;
            isGrounded = false;

            RaycastObstacle(pos_high + GetFacing() * (grab_dist_x + 0.1f), Vector3.down, ground_filter, out dirY);

            float dirYOffset = grab_dist_up - dirY;
            Vector3 npos = new Vector3(transform.position.x + GetFacingSide() * dirX, transform.position.y + dirYOffset, 0f);
            transform.position = npos;
            grab_pos = ledge_grab_root.transform.position;
        }
    }

    private void UpdateLedgeGrab()
    {
        if (state_timer > 0.2f)
        {
            if (controls.GetMovePress().y > 0.1f)
            {
                state = PlayerCharacterState.LedgeGrabUp;
                state_timer = 0f;
                collide.isTrigger = true;
                anim.SetTrigger("LedgeClimb");
            }
            else if (controls.GetMovePress().y < -0.1f)
            {
                state = PlayerCharacterState.Normal;
                state_timer = 0f;
                collide.isTrigger = false;
            }

            if (controls.IsPressJump())
                Jump();
        }
    }

    private void UpdateLedgeGrabUp()
    {
        Vector3 target = grab_pos + Vector3.up * grab_climb_distance + Vector3.right * grab_climb_distance * GetFacingSide();
        float dist = (transform.position - target).magnitude;

        if (collide.isTrigger)
            transform.position = Vector3.MoveTowards(transform.position, target, 4f * Time.deltaTime);

        if (dist < 0.1f)
            collide.isTrigger = false;

        if (state_timer > grab_climb_duration)
        {
            state = PlayerCharacterState.Normal;
            state_timer = 0f;
            collide.isTrigger = false;
        }
    }

    private bool DetectTopped()
    {
        float factor = isCrouch ? (1 / crouch_collider_factor) : 1f;
        Vector3 pos = GetCapsulePos(Vector3.up) + Vector3.up * GetCapsuleRadius() * factor;
        float radius = ground_radius * factor;
        Debug.DrawRay(pos, Vector3.up * radius, Color.red);
        return RaycastObstacle(pos, Vector2.up * radius, ground_filter);
    }

    private bool DetectOverlapObstacle()
    {
        Vector3 pos = collide.bounds.center;
        float radius = GetCapsuleRadius();
        //Debug.DrawRay(pos, Vector3.right * radius, Color.red);
        return Physics2D.OverlapCircle(pos, radius, ground_layer.value | ground_layer_semi.value);
    }

    private bool DetectGrounded(int layerMask)
    {
        Vector3 pos = base_root.transform.position;
        float radius = ground_radius;
        //Debug.DrawRay(pos, Vector3.down * radius, Color.white);
        return Physics2D.OverlapCircle(pos, radius, layerMask);
    }

    private GameObject DetectGroundedTag(ContactFilter2D filter, string tag)
    {
        Vector3 pos = base_root.transform.position;
        float radius = ground_radius * 2f;
        //Debug.DrawRay(pos, Vector3.down * radius, Color.white);
        Collider2D[] results = new Collider2D[4];
        int nb = Physics2D.OverlapCircle(pos, radius, filter, results);
        for (int j = 0; j < nb; j++)
        {
            if (results[j] != null && results[j] != collide)
            {
                if (results[j].tag == tag)
                    return results[j].gameObject;
            }
        }
        return null;
    }

    private bool DetectFronted(ContactFilter2D filter) {
        float front_dist = GetCapsuleRadius();
        float radius = GetCapsuleRadius() - 0.05f;
        Vector3 side_vect = Vector3.right * GetFacingSide() * front_dist;
        //ContactFilter2D filter = go_through_semi ? ground_filter : ground_filter_both;
        return //RaycastObstacle(GetCapsulePos(Vector3.up), Vector3.right * GetFacingSide() * front_dist, filter) 
               //|| RaycastObstacle(GetCapsulePos(Vector3.down), Vector3.right * GetFacingSide() * front_dist, filter)
               //|| RaycastObstacle(GetCapsulePos(Vector2.zero), Vector3.right * GetFacingSide() * front_dist, filter)
            OverlapObstacle(GetCapsulePos(Vector3.up) + side_vect, radius, filter.layerMask)
            || OverlapObstacle(GetCapsulePos(Vector3.down) + side_vect, radius, filter.layerMask)
            || OverlapObstacle(GetCapsulePos(Vector3.zero) + side_vect, radius, filter.layerMask)
            || RaycastObstacle(GetCapsulePos(Vector3.up) + Vector3.up * radius, Vector3.right * GetFacingSide() * front_dist, filter)
            || RaycastObstacle(GetCapsulePos(Vector3.down) + Vector3.down * radius, Vector3.right * GetFacingSide() * front_dist, filter);
    }

    private bool DetectFrontedSemi(ContactFilter2D filter)
    {
        Vector3 center = semi_collider.transform.position;
        Vector3 radius = GetSemiSize();
        Vector3 side_vect = Vector3.right * GetFacingSide() * (radius.x + 0.11f);

        Debug.DrawRay(center, side_vect);
        Debug.DrawRay(center + Vector3.up * radius.y, side_vect);
        Debug.DrawRay(center + Vector3.down * radius.y, side_vect);

        return RaycastObstacle(center, side_vect, filter)
            || RaycastObstacle(center + Vector3.up * radius.y, side_vect, filter);
            //|| RaycastObstacle(center + Vector3.down * radius.y, side_vect, filter);
    }

    private bool OverlapObstacle(Vector3 center, float radius, int layer_mask)
    {
        //Debug.DrawRay(center, Vector3.up * radius, Color.blue);
        return Physics2D.OverlapCircle(center, radius, layer_mask);
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

    private bool RaycastObstacle(Vector2 pos, Vector2 dir, ContactFilter2D filter, out float odist)
    {
        RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
        //Debug.DrawRay(pos, dir);
        Physics2D.Raycast(pos, dir.normalized, filter, hitBuffer, dir.magnitude);
        for (int j = 0; j < hitBuffer.Length; j++)
        {
            if (hitBuffer[j].collider != null && hitBuffer[j].collider != collide && !hitBuffer[j].collider.isTrigger)
            {
                odist = hitBuffer[j].distance;
                return true;
            }
        }
        odist = 0f;
        return false;
    }

    private void OnHitEnemy(PlayerAttack attack, Enemy enemy) {

        if (enemy.CanBeHit() && !hit_enemy_list.Contains(enemy))
        {
            if (attack.mana < 0)
                AddMana(-attack.mana);
            hit_enemy_list.Add(enemy);
            enemy.TakeDamage(attack.damage);
            Vector3 dir = (enemy.transform.position - transform.position).normalized;
            Vector3 knock = new Vector3(Mathf.Sign(dir.x) * attack.knockback.x, attack.knockback.y);
            enemy.PushBack(knock);
        }
    }

    private Vector3 GetCapsulePos(Vector3 dir)
    {
        Vector2 orientation = dir.normalized;
        Vector2 raycast_offset = new Vector2(collide.offset.x * GetFacingSide(), collide.offset.y) + orientation * Mathf.Abs(collide.size.y * 0.5f - collide.size.x * 0.5f);
        return rigid.position + raycast_offset * collide.transform.lossyScale.y;
    }

    private float GetCapsuleRadius()
    {
        return GetSize().x * 0.5f + collide.edgeRadius;
    }

    public Vector2 GetSize()
    {
        return new Vector2(Mathf.Abs(collide.transform.lossyScale.x) * collide.size.x, Mathf.Abs(collide.transform.lossyScale.y) * collide.size.y);
    }

    private Vector3 GetSemiSize()
    {
        return new Vector2(Mathf.Abs(collide.transform.lossyScale.x) * semi_collider.size.x, Mathf.Abs(collide.transform.lossyScale.y) * semi_collider.size.y);
    }

    public PlayerAttack GetAttack(string id)
    {
        foreach (PlayerAttack attack in attacks)
        {
            if (attack.id == id)
                return attack;
        }
        return null;
    }

    public Vector3 GetMove()
    {
        return move;
    }

    public Vector3 GetFacing()
    {
        return facing;
    }

    public float GetFacingSide()
    {
        return Mathf.Sign(facing.x);
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    public bool IsCrouching()
    {
        return isCrouch;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsFronted()
    {
        return isFronted;
    }

    public int GetHP()
    {
        return hp;
    }

    public int GetMana()
    {
        return mana;
    }

    public bool IsDead()
    {
        return state == PlayerCharacterState.Dead;
    }

    public static PlayerCharacter Get()
    {
        return _instance;
    }
}
