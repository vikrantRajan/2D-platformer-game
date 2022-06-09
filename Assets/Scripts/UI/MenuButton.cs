using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic Menu button script (for handling with keyboard/gamepad)
/// </summary>

public class MenuButton : ObjectUI
{
    private Button button;

    protected override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
    }

    protected override void Accept()
    {
        button.onClick.Invoke();
    }

}
