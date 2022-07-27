using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllableLight : MonoBehaviour
{
    //[Range(0, 100)]public float state = 0;


    public float startIntensity;
    public float endIntensity;

    public Color startColor;
    public Color endColor;


    private float lightAdjustDuration = 5.0f;

    private Light thisLight;

    private Color currentColorTarget;
    private float currentIntensityTarget;
    private Color currentColor;
    private float currentIntensity;


    private float state;
    private float animator = 0;

    // Start is called before the first frame update
    void Start()
    {
        thisLight = GetComponent<Light>();

        GameEvents.Instance.onTrashCountChange += onTrashStateChange;
        lightAdjustDuration = GameEvents.Instance.lightAnimationDuration;

        thisLight.intensity = Mathf.Lerp(endIntensity, startIntensity, 1);
        thisLight.color = Color.Lerp(endColor, startColor, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(animator < 1)
        {
            animator += Time.deltaTime/lightAdjustDuration;

            thisLight.intensity = Mathf.Lerp(currentIntensity, currentIntensityTarget, animator);
            thisLight.color = Color.Lerp(currentColor, currentColorTarget, animator);
            //Debug.Log(animator);

        }


    }

    //Event for the state changed
    private void onTrashStateChange(float stateIn)
    {
        //Animation will start, if animator variable is > 1
        animator = 0;
        state = stateIn;
        //Debug.Log("in lampe" + stateIn);
        currentIntensity = thisLight.intensity;
        currentColor = thisLight.color;
        currentIntensityTarget = Mathf.Lerp(endIntensity, startIntensity, state);
        currentColorTarget = Color.Lerp(endColor, startColor, state);
    }
}
