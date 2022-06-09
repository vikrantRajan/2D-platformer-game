using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class PlayerControls : MonoBehaviour
{
    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode down;

    public KeyCode jump;
    public KeyCode melee;
    public KeyCode magic;

    public KeyCode jump2;
    public KeyCode melee2;
    public KeyCode magic2;

    public KeyCode accept;
    public KeyCode cancel;
    public KeyCode pause;

    public InputControlType gamepad_jump = InputControlType.Action1;
    public InputControlType gamepad_melee = InputControlType.Action3;
    public InputControlType gamepad_magic = InputControlType.Action4;

    public InputControlType gamepad_accept = InputControlType.Action1;
    public InputControlType gamepad_cancel = InputControlType.Action2;
    public InputControlType gamepad_pause = InputControlType.Command;

    private Vector2 move;
    private Vector2 move_press;
    private bool is_jump;
    private bool is_melee;
    private bool is_shoot;
    private bool is_jump_hold;
    private bool is_accept;
    private bool is_cancel;
    private bool is_pause;

    private InputDevice active_device;

    private static PlayerControls _instance;

    void Awake()
    {
        _instance = this;
    }
    
    void Update()
    {
        move = Vector2.zero;

        if (Input.GetKey(left))
            move += Vector2.left;
        if (Input.GetKey(right))
            move += Vector2.right;
        if (Input.GetKey(down))
            move += Vector2.down;
        if (Input.GetKey(up))
            move += Vector2.up;

        if (Input.GetKey(KeyCode.LeftArrow))
            move += Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow))
            move += Vector2.right;
        if (Input.GetKey(KeyCode.DownArrow))
            move += Vector2.down;
        if (Input.GetKey(KeyCode.UpArrow))
            move += Vector2.up;

        move_press = Vector2.zero;
        if (Input.GetKeyDown(left))
            move_press += Vector2.left;
        if (Input.GetKeyDown(right))
            move_press += Vector2.right;
        if (Input.GetKeyDown(down))
            move_press += Vector2.down;
        if (Input.GetKeyDown(up))
            move_press += Vector2.up;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            move_press += Vector2.left;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            move_press += Vector2.right;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            move_press += Vector2.down;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            move_press += Vector2.up;

        is_jump = Input.GetKeyDown(jump) || Input.GetKeyDown(jump2);
        is_jump_hold = Input.GetKey(jump) || Input.GetKey(jump2);
        is_melee = Input.GetKeyDown(melee) || Input.GetKeyDown(melee2);
        is_shoot = Input.GetKeyDown(magic) || Input.GetKeyDown(magic2);
        is_accept = Input.GetKeyDown(accept);
        is_cancel = Input.GetKeyDown(cancel);
        is_pause = Input.GetKeyDown(pause);

        active_device = InputManager.ActiveDevice;

        if(active_device != null)
        {
            is_jump = is_jump || active_device.GetControl(gamepad_jump).WasPressed;
            is_jump_hold = is_jump_hold || active_device.GetControl(gamepad_jump).IsPressed;
            is_melee = is_melee || active_device.GetControl(gamepad_melee).WasPressed;
            is_shoot = is_shoot || active_device.GetControl(gamepad_magic).WasPressed;
            is_accept = is_accept || active_device.GetControl(gamepad_accept).WasPressed;
            is_cancel = is_cancel || active_device.GetControl(gamepad_cancel).WasPressed;
            is_pause = is_pause || active_device.GetControl(gamepad_pause).WasPressed;

            move = move + GetTwoAxis(active_device, InputControlType.LeftStickX, InputControlType.LeftStickY);
            move_press = move_press
                + GetTwoAxisThreshold(active_device, InputControlType.LeftStickX, InputControlType.LeftStickY)
                + GetTwoAxisPress(active_device, InputControlType.DPadX, InputControlType.DPadY);
        }

        move = move.normalized * Mathf.Min(move.magnitude, 1f);
        move_press = move_press.normalized * Mathf.Min(move_press.magnitude, 1f);
    }

    private float GetAxis(InputDevice device, InputControlType type)
    {
        if (device != null)
            return device.GetControl(type).Value;
        return 0f;
    }

    private Vector2 GetTwoAxis(InputDevice device, InputControlType typeX, InputControlType typeY)
    {
        return new Vector2(GetAxis(device, typeX), GetAxis(device, typeY));
    }

    private float GetAxisPress(InputDevice device, InputControlType type)
    {
        if (device != null)
        {
            InputControl control = device.GetControl(type);
            return control.WasPressed ? control.Value : 0f;
        }
        return 0f;
    }

    private Vector2 GetTwoAxisPress(InputDevice device, InputControlType typeX, InputControlType typeY)
    {
        return new Vector2(GetAxisPress(device, typeX), GetAxisPress(device, typeY));
    }

    private float GetAxisThreshold(InputDevice device, InputControlType type)
    {
        if (device != null)
        {
            InputControl control = device.GetControl(type);
            return Mathf.Abs(control.LastValue) < 0.5f && Mathf.Abs(control.Value) >= 0.5f ? Mathf.Sign(control.Value) : 0f;
        }
        return 0f;
    }

    private Vector2 GetTwoAxisThreshold(InputDevice device, InputControlType typeX, InputControlType typeY)
    {
        return new Vector2(GetAxisThreshold(device, typeX), GetAxisThreshold(device, typeY));
    }

    public Vector2 GetMove()
    {
        return move;
    }

    public Vector2 GetMovePress()
    {
        return move_press;
    }

    public bool IsPressJump()
    {
        return is_jump;
    }

    public bool IsPressMelee()
    {
        return is_melee;
    }

    public bool IsPressShoot()
    {
        return is_shoot;
    }

    public bool IsHoldJump()
    {
        return is_jump_hold;
    }

    public bool IsAccept()
    {
        return is_accept;
    }

    public bool IsCancel()
    {
        return is_cancel;
    }

    public bool IsPause()
    {
        return is_pause;
    }

    public bool HasGamePad()
    {
        return active_device != null;
    }

    public bool IsMenuPressLeft() { return move_press.x < -0.5f; }
    public bool IsMenuPressRight() { return move_press.x > 0.5f; }
    public bool IsMenuPressUp() { return move_press.y > 0.5f; }
    public bool IsMenuPressDown() { return move_press.y < -0.5f; }

    public static PlayerControls Get()
    {
        return _instance;
    }
}
