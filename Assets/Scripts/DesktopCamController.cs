using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DesktopCamController : MonoBehaviour
{

    public float translateDamping = 15;
    public GameObject PlayerHMD;
    public Transform topdownTransform;
    public string[] layersToIgnoreInTopdown;


    //Orbit Variables
    public float TopDownCamPivotDistance = 20.0f;
    public float ScrollDampening = 6f;
    public float OrbitDamping = 10f;
    public float TranslateSensitivity = 0.25f;
    public float RotationSensitivity = 2.0f;
    public float ScrollSensitivity = 0.3f;
    private Vector3 DestinedRotation;
    private Vector3 DestinedTranslation;

    private bool use2D = false;
    Transform currentViewTransform;
    Camera spectatorCam;

    void Start()
    {
        spectatorCam = this.gameObject.GetComponent<Camera>();
        currentViewTransform = PlayerHMD.transform;
    }

    private void Update()
    {
        // Change to Topdown
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTopdown();
        }
        // Change to VR folowing Cam
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetVRFollow();
        }
        
    }

    public void SwichtoView()
    {
        if (!use2D)
            SetTopdown();
        else
            SetVRFollow();
    }

    void LateUpdate()
    {
        // Lerp from 2D to VR-follow
        if(!use2D)
        {
            transform.position = Vector3.Lerp(transform.position, currentViewTransform.position, Time.deltaTime * translateDamping);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(currentViewTransform.rotation.eulerAngles), Time.deltaTime * translateDamping);
        }
        else
        {
            //Inputs:
            
            if(Input.GetKey(KeyCode.Mouse0)) //Left Mouse Button -> Rotation
            {
                if(Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
                {
                    DestinedRotation.x -= Input.GetAxis("Mouse Y") * RotationSensitivity;
                    DestinedRotation.y += Input.GetAxis("Mouse X") * RotationSensitivity;

                    DestinedRotation.x = Mathf.Clamp(DestinedRotation.x, 5.0f, 90.0f);
                }
            }
            else if(Input.GetKey(KeyCode.Mouse1)) //Right Mouse Button -> Translation
            {
                if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
                {
                    Vector3 deltaCamTranslation = new Vector3(
                        - Input.GetAxis("Mouse X"),
                        - Input.GetAxis("Mouse Y"),
                        0f);
                    deltaCamTranslation = transform.parent.rotation * deltaCamTranslation;

                    DestinedTranslation += deltaCamTranslation * TranslateSensitivity;

                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0f) //Zoom
            {
                float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity * TopDownCamPivotDistance;
                TopDownCamPivotDistance += scrollAmount * -1f;
                TopDownCamPivotDistance = Mathf.Clamp(TopDownCamPivotDistance, 1.5f, 30f);
            }


            //Actual Camera Transform
            //Cam translation 
            transform.parent.position = Vector3.Lerp(transform.parent.position, DestinedTranslation, Time.deltaTime * translateDamping);


            //Cam rotation:
            Quaternion destinedRotation = Quaternion.Euler(DestinedRotation.x, DestinedRotation.y, 0);
            transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, destinedRotation, Time.deltaTime * OrbitDamping);

            //Cam distance:
            if(transform.localPosition.z != TopDownCamPivotDistance * -1)
                    transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(transform.localPosition.z, TopDownCamPivotDistance * -1f, Time.deltaTime * ScrollDampening));

        }
    }

    void SetTopdown()
    {
        DestinedTranslation = new Vector3(
            PlayerHMD.transform.position.x,
            0,
            PlayerHMD.transform.position.z
            );

        DestinedRotation.x = 90f;
        DestinedRotation.y = 0f;
        DestinedRotation.z = 0f;
        Quaternion rot = PlayerHMD.transform.rotation;
        topdownTransform.SetPositionAndRotation(PlayerHMD.transform.position, rot);

        //Reparent Cam to Scene, second Param is true to keep World Transform and lerp petween it and the destined transformation!
        transform.SetParent(topdownTransform, false);
        use2D = true;
        ToggleCullingMask(false);
    }

    void SetVRFollow()
    {
        use2D = false;
        currentViewTransform = PlayerHMD.transform;
        //Reparent Cam to HMD
        transform.SetParent(PlayerHMD.transform, true);
        ToggleCullingMask(true);
    }

    void ToggleCullingMask(bool showLayers)
    {
        foreach(string s in layersToIgnoreInTopdown)
        {
            if(showLayers)
                spectatorCam.cullingMask |= 1 << LayerMask.NameToLayer(s); //Turn on layer-Mask
            else
                spectatorCam.cullingMask &= ~(1 << LayerMask.NameToLayer(s)); //Turn off layer Mask
        }
    }

    public void UpdateRotateSensitivity(float value)
    {
        RotationSensitivity = value;
    }

    public void UpdateTransitionSensitivity(float value)
    {
        TranslateSensitivity = value;
    }
}
