using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorForTrash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().materials[0].SetColor("Albedo", new Color(1, 1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
