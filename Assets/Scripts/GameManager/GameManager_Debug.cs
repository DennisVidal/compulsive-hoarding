using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Debug : MonoBehaviour
{

    [Range(0, 1)] public float trashState;

    private float trashStateChanged;

    private void Start()
    {
        trashStateChanged = trashState;

        //This is for debug only replace "controllableLight" with the trash script

        controllableLight[] test = FindObjectsOfType(typeof(controllableLight)) as controllableLight[];

        Debug.Log("Found " + test.Length + " instances of lights");

    }

    public void Update()
    {
        if (trashState != trashStateChanged)
        {
            //Trigger event
            GameEvents.Instance.TrashCountChange(trashState);
            trashStateChanged = trashState;
        }
    }
}
