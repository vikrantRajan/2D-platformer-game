using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic Menu script (for handling with keyboard/gamepad)
/// </summary>

public class ObjectUI : MonoBehaviour
{
    [Header("ObjectUI")]
    public string group;
    public GameObject menu_arrow;
    public bool starting = false;
    
    private RectTransform rect;
    private RectTransform arrow;

    private int index;

    private bool is_leader = false;
    private int selection = 0;
    private float height;
    private int start_index = 0;
    private List<ObjectUI> group_list = new List<ObjectUI>();

    private static List<ObjectUI> object_list = new List<ObjectUI>();

    protected virtual void Awake()
    {
        if (GetGroup(group).Count == 0)
            is_leader = true;

        object_list.Add(this);
        rect = GetComponent<RectTransform>();
        height = rect.anchoredPosition.y;

        if (menu_arrow != null)
        {
            GameObject arro = Instantiate(menu_arrow, transform);
            arrow = arro.GetComponent<RectTransform>();
            arrow.anchoredPosition = Vector2.left * (rect.sizeDelta.x * 0.5f + arrow.sizeDelta.x * 0.5f);
            arrow.gameObject.SetActive(false);
        }
    }

    protected virtual void OnDestroy()
    {
        object_list.Remove(this);
    }

    protected virtual void Start()
    {
        if (is_leader)
        {
            group_list = GetGroup(group);
            group_list.Sort((p1, p2) =>
            {
                return p2.height.CompareTo(p1.height);
            });

            for (int i = 0; i < group_list.Count; i++) {
                group_list[i].index = i;
                if (group_list[i].starting)
                    selection = i;
            }

            ObjectUI start = GetGroupStart(group);
            start_index = start ? start.index : 0;
        }
    }

    protected virtual void Update()
    {
        if (!PlayerControls.Get().HasGamePad())
            return;

        if (is_leader)
        {
            if (IsVisible())
            {
                PlayerControls controls = PlayerControls.Get();

                if (controls.IsMenuPressUp())
                {
                    selection--;
                    selection = Mathf.Clamp(selection, 0, group_list.Count - 1);
                }

                if (controls.IsMenuPressDown())
                {
                    selection++;
                    selection = Mathf.Clamp(selection, 0, group_list.Count - 1);
                }

                if (controls.IsMenuPressLeft())
                {
                    if (selection >= 0 && selection < group_list.Count)
                    {
                        ObjectUI button = group_list[selection];
                        button.PressLeft();
                    }
                }

                if (controls.IsMenuPressRight())
                {
                    if (selection >= 0 && selection < group_list.Count)
                    {
                        ObjectUI button = group_list[selection];
                        button.PressRight();
                    }
                }

                if (controls.IsAccept())
                {
                    if (selection >= 0 && selection < group_list.Count)
                    {
                        ObjectUI button = group_list[selection];
                        button.Accept();
                    }
                }

                foreach (ObjectUI button in group_list)
                {
                    button.SetArrow(selection == button.index);
                }
            }
            else
            {
                selection = start_index;
            }
        }
    }

    protected virtual void Accept()
    {

    }

    protected virtual void PressLeft()
    {

    }

    protected virtual void PressRight()
    {

    }

    public void SetArrow(bool visible)
    {
        if (arrow != null)
            arrow.gameObject.SetActive(visible);
    }

    public bool IsVisible(){
        return gameObject.activeInHierarchy;
    }

    public static ObjectUI GetGroupStart(string group)
    {
        foreach (ObjectUI button in object_list)
        {
            if (button.group == group && button.starting)
                return button;
        }
        return null;
    }

    public static List<ObjectUI> GetGroup(string group)
    {
        List<ObjectUI> valid_button = new List<ObjectUI>();
        foreach (ObjectUI button in object_list)
        {
            if (button.group == group)
                valid_button.Add(button);
        }
        return valid_button;
    }
}
