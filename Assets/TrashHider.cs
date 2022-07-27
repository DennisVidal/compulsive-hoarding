using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TrashHider : MonoBehaviour
{
    public bool l1 = true;
    public List<GameObject> level1;
    public bool l2 = true;
    public List<GameObject> level2;
    public bool l3 = true;
    public List<GameObject> level3;
    public float trashstate;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.onTrashCountChange += onTrashStateChange;
        trashstate = 1.0f;
        setActives();
    }

    private void onTrashStateChange(float stateIn)
    {
        trashstate = stateIn;
        setActives();   
    }

    private void setActives()
    {
        if (trashstate > 0.66f) l3 = true;
        else l3 = false;

        if (trashstate > 0.33f) l2 = true;
        else l2 = false;

        if (trashstate > 0.1f) l1 = true;
        else l1 = false;

        foreach (GameObject go in level3)
        {
            go.SetActive(l3);
        }
        foreach (GameObject go in level2)
        {
            go.SetActive(l2);
        }
        foreach (GameObject go in level1)
        {
            go.SetActive(l1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
