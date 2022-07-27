using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuellTonnenTrigger : MonoBehaviour
{
    public GameObject sliderHandle;
    private int triggerCounter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //triggerCounter++;
        //if (triggerCounter == 1)
        //{
        //    sliderHandle.GetComponent<SliderTopScript>().Open();
        //}

        if (other.CompareTag("Player"))
        {
            sliderHandle.GetComponent<SliderTopScript>().Open();
        }

        
        
        //Debug.Log("On Enter TriggerCounter = " + triggerCounter);
    }

    private void OnTriggerExit(Collider other)
    {
        //triggerCounter--;
        //if (triggerCounter == 0)
        //{
        //    sliderHandle.GetComponent<SliderTopScript>().Close();
        //}
        //Debug.Log("On Exit TriggerCounter = " + triggerCounter);

        if (other.CompareTag("Player"))
        {
            sliderHandle.GetComponent<SliderTopScript>().Close();
        }
    }

}
