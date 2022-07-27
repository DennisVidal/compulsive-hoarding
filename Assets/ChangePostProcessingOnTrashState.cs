using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class ChangePostProcessingOnTrashState : MonoBehaviour
{
    private PostProcessVolume m_Volume;
    private PostProcessProfile m_Profile;
    private ColorGrading m_ColorGrading;

    private float state;

    public float startValue = -0.2f;
    public float endValue = 1.5f;

    //Stuff for animation
    private float animator = 0;
    private float adjustDuration = 5.0f;
    private float currentValue;
    private float currentValueTarget;


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.onTrashCountChange += onTrashStateChange;
        adjustDuration = GameEvents.Instance.lightAnimationDuration;

        m_Volume = this.GetComponent<PostProcessVolume>();
        m_Volume.profile.TryGetSettings<ColorGrading>(out m_ColorGrading);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (animator < 1)
        {
            //Debug.Log("Animator: " + animator);
            animator += Time.deltaTime / 5.0f;
            //Debug.Log("Animator: " + animator);
            m_ColorGrading.postExposure.value = Mathf.Lerp(currentValue, currentValueTarget, animator);
        }
    }

    //Event for the statechangeasd
    private void onTrashStateChange(float stateIn)
    {
        animator = 0;

        currentValue = m_ColorGrading.postExposure.value;
        currentValueTarget = Mathf.Lerp(endValue, startValue, stateIn);
    }
}
