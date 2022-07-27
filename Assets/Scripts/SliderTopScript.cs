using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderTopScript : MonoBehaviour
{
    private bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //TODO make animation for those
    public void Open()
    {
        if (!isOpen)
        {
            GetComponent<Animator>().Play("Container", 0, 0.0f);
            isOpen = true;
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            GetComponent<Animator>().Play("ContainerClose", 0, 0.0f);
            isOpen = false;
        }
    }
}
