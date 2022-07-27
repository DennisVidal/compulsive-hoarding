using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceConnector : MonoBehaviour
{
    public Slider trashSlider;
    public Slider clearSlider; // defines how many objects have to be thrown away to be finished
    public void ButtonPressStartTherapy()
    {
        GameEvents.Instance.ButtonPressStartTherapy((int)clearSlider.value);
    }

    public void ButtonPressStopTherapy()
    {
        GameEvents.Instance.ButtonPressStopTherapy();
    }

    public void ButtonPressSpawn()
    {
        GameEvents.Instance.ButtonPressSpawn((int)trashSlider.value);
        clearSlider.maxValue = trashSlider.value;
    }
    public void ButtonPressEndGame()
    {
        GameEvents.Instance.ButtonPressEndGame();
    }

    public void ButtonPressSpawnSettings()
    {
        GameEvents.Instance.ButtonPressSpawnSettings();
    }

    public void ButtonPressOpenCredits()
    {
        GameEvents.Instance.ButtonPressOpenCredits();
    }
    
}
