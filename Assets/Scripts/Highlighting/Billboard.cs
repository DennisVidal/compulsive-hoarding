using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    [Tooltip("Camera the billboard will be rotated towards.")]
    public Camera cameraToLookAt;


    [Tooltip("Should the billboard be limited on its rotation on the x-axis?")]
    public bool limitXAxis = false;

    private void Start()
    {
        if (!cameraToLookAt)
        {
            cameraToLookAt =GameEvents.Instance.VrCam.GetComponent<Camera>();
        }
    }

    protected void Update()
    {
        if(cameraToLookAt)
        {
            Vector3 targetPosition = cameraToLookAt.transform.position;

            if(limitXAxis)
            {
                targetPosition[1] = transform.position[1];
            }
                        
            transform.LookAt(targetPosition);
        }
    }
}
