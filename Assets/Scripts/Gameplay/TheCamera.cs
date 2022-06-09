using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheCamera : MonoBehaviour
{
    public bool isFollowing;

    public float updateSpeedX = 20f;
    public float updateSpeedY = 4f;
    public float updateSpeedY_jump = 2f;

    public float xOffset = 0f;
    public float yOffset = 0f;

    private Camera cam;
    private Vector3 start_pos;

    private float average_pos_x;
    private float average_pos_y;
    private float current_speed_y;

    private bool lock_camera = false;
    private GameObject lock_target = null;
    private Vector3 custom_offset = Vector3.zero;

    private Vector3 shake_vector = Vector3.zero;
    private float shake_timer = 0f;
    private float shake_intensity = 1f;

    private static TheCamera _instance;
    
    void Awake()
    {
        _instance = this;
        cam = GetComponent<Camera>();
        start_pos = transform.position;
        average_pos_x = transform.position.x;
        average_pos_y = transform.position.y;
    }

    private void Start()
    {
        if (isFollowing)
        {
            PlayerCharacter player = PlayerCharacter.Get();
            if(player)
                MoveToTarget(player.transform.position);
        }
    }

    void LateUpdate()
    {

        if(isFollowing)
        {
            PlayerCharacter player = PlayerCharacter.Get();

            if (!lock_camera || lock_target != null)
            {
                if (!lock_camera && player.IsJumping())
                    current_speed_y = updateSpeedY_jump;
                current_speed_y = Mathf.MoveTowards(current_speed_y, (!lock_camera && player.IsJumping() ? updateSpeedY_jump : updateSpeedY), 2f * Time.deltaTime);

                GameObject target = lock_camera && lock_target != null ? lock_target : player.gameObject;

                average_pos_x = Mathf.MoveTowards(average_pos_x, target.transform.position.x, updateSpeedX * Time.deltaTime);
                average_pos_y = Mathf.MoveTowards(average_pos_y, target.transform.position.y, current_speed_y * Time.deltaTime);

                Vector3 targ_pos = new Vector3(average_pos_x + xOffset + custom_offset.x, average_pos_y + yOffset + custom_offset.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targ_pos, 20f * Time.deltaTime);
            }
        }

        //Shake FX
        if (shake_timer > 0f)
        {
            shake_timer -= Time.deltaTime;
            shake_vector = new Vector3(Mathf.Cos(shake_timer * Mathf.PI * 8f) * 0.02f, Mathf.Sin(shake_timer * Mathf.PI * 7f) * 0.02f, 0f);
            transform.position += shake_vector * shake_intensity;
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        transform.position = new Vector3(target.x, target.y, transform.position.z);
        start_pos = target;
        average_pos_x = transform.position.x;
        average_pos_y = transform.position.y;
    }

    public void SetCameraOffset(Vector3 offset)
    {
        custom_offset = offset;
    }

    public void LockCamera(GameObject target)
    {
        lock_camera = true;
        lock_target = target;
    }

    public void UnlockCamera()
    {
        lock_camera = false;
        lock_target = null;
    }

    public void Shake(float intensity = 2f, float duration = 0.5f)
    {
        shake_intensity = intensity;
        shake_timer = duration;
    }

    public Camera GetCamera()
    {
        return cam;
    }

    public static TheCamera Get()
    {
        return _instance;
    }
}
