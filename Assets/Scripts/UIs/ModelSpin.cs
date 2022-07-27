using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSpin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 euler = new Vector3(90, 60, 30);
        euler = euler * Time.deltaTime;
        transform.Rotate(euler);
    }
}
