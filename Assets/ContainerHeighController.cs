using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerHeighController : MonoBehaviour
{
    public float percentDebug;
    //private Vector3 topPos = new Vector3(-0.61969f, 0.352895f, -0.122f);
    float top = 0.352895f;
    float bot = -0.546f;
    float zero;
    //private Vector3 botPos = new Vector3(-0.61969f, -0.546f, -0.122f);

    // Start is called before the first frame update
    void Start()
    {
        zero = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        SetHeightInPercentage(percentDebug);
    }

    public void SetHeightInPercentage(float percentage)
    {
        float l = Mathf.Lerp(bot, top, percentage);
        transform.localPosition = new Vector3(transform.localPosition.x, l, transform.localPosition.z);
        float scaleFactor = Mathf.Lerp(0.890522f, 1.0f, percentage);
        transform.localScale = new Vector3(scaleFactor, 1.0f, 1.0f); 
    }
}
