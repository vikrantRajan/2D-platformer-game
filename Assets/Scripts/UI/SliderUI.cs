using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderUI : ObjectUI
{
    [Header("SliderUI")]
    public Text value_text;
    public int step_value = 10;

    public UnityAction<float> onChange;

    private Slider slider;

    protected override void Awake()
    {
        base.Awake();
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnChange);
    }

    protected override void Update()
    {
        base.Update();
        value_text.text = Mathf.RoundToInt(slider.value).ToString();
    }

    public void SetValue(float val)
    {
        slider.value = val;
    }

    public void SetMax(float max)
    {
        slider.maxValue = max;
    }

    public float GetValue()
    {
        return slider.value;
    }

    protected override void PressLeft()
    {
        slider.value -= step_value;

    }

    protected override void PressRight()
    {
        slider.value += step_value;

    }

    private void OnChange(float val)
    {
        if (onChange != null)
            onChange.Invoke(val);
    }
}
