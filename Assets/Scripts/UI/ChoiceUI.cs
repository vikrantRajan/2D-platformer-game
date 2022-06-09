using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoiceUI : ObjectUI
{
    [Header("ChoiceUI")]
    public Text text;

    public UnityAction<string> onChange;

    private int choice_index = 0;

    private Dictionary<string, string> choices = new Dictionary<string, string>();
    private List<string> choices_index = new List<string>();

    public void AddChoice(string value)
    {
        choices.Add(value, value);
        choices_index.Add(value);
    }

    public void AddChoice(string value, string text)
    {
        choices.Add(value, text);
        choices_index.Add(value);
    }

    public void ResetChoice()
    {
        choices.Clear();
        choices_index.Clear();
        text.text = "";
        choice_index = -1;
    }

    public void SetTo(string choice)
    {
        choice_index = choices_index.IndexOf(choice);

        text.text = choices[choice];

        if (onChange != null)
            onChange.Invoke(choice);
    }

    public void SetTo(int ind)
    {
        choice_index = ind;
        choice_index = Mathf.Clamp(choice_index, 0, choices.Count - 1);
        string id = choices_index[choice_index];
        SetTo(id);
    }

    public void MoveNext(int side)
    {
        choice_index += side;
        choice_index = Mathf.Clamp(choice_index, 0, choices.Count - 1);
        string id = choices_index[choice_index];
        SetTo(id);
    }

    protected override void PressLeft()
    {
        MoveNext(-1);

    }

    protected override void PressRight()
    {
        MoveNext(1);

    }

    public string GetSelectedText()
    {
        string id = GetSelectedValue();
        if(choices.ContainsKey(id))
            return choices[id];
        return "";
    }

    public string GetSelectedValue()
    {
        if (choice_index < choices_index.Count)
            return choices_index[choice_index];
        return "";
    }

    public int GetSelectedIndex()
    {
        return choice_index;
    }
}
