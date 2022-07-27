using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCanObjectCounter : MonoBehaviour
{
    private int numOfObjects;
    // Start is called before the first frame update
    void Start()
    {
        numOfObjects = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void initColliderIgnores()
    //{
    //    BoxCollider collider = this.GetComponent<BoxCollider>();

    //}

    private void OnTriggerEnter(Collider other)
    {
        numOfObjects++;
        //Debug.Log("ENTER : numOfObects = " + numOfObjects);
    }

    private void OnTriggerExit(Collider other)
    {
        numOfObjects--;
        //Debug.Log("EXIT : numOfObects = " + numOfObjects);
    }


}
