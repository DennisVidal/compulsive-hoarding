using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueHolder : MonoBehaviour
{
    public float trashvalue = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTrashValue(float value)
    {
        trashvalue = value;
    }
}
