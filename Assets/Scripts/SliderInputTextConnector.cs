using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderInputTextConnector : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public InputField inputField;
    void Start()
    {
        SliderUpdated();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SliderUpdated()
    {
        inputField.SetTextWithoutNotify(((int)slider.value).ToString());
    }

    public void InputFieldUpdated()
    {
        //Clamp value
        if (int.Parse(inputField.text) < 1)
            inputField.SetTextWithoutNotify("1");
        if (int.Parse(inputField.text) > slider.value)
            inputField.SetTextWithoutNotify(((int)slider.maxValue).ToString());

        slider.value = int.Parse(inputField.text);
    }
}
