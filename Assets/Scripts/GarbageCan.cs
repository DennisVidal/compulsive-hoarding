using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCan : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject slider;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void OpenSilder()
    {
        slider.GetComponent<SliderTopScript>().Open();
    }

    public void CloseSlider()
    {
        slider.GetComponent<SliderTopScript>().Close();
    }
}
