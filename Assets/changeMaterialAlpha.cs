using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMaterialAlpha : MonoBehaviour
{
    public float startAlpha;
    public float endAlpha;

    private Color color;
    private MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.onTrashCountChange += onTrashStateChange;
        renderer = GetComponent<MeshRenderer>();
        color = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Event for the statechangeasd
    private void onTrashStateChange(float stateIn)
    {
        color.a = Mathf.Lerp(endAlpha, startAlpha, stateIn);
        renderer.material.color = color;
    }
}
