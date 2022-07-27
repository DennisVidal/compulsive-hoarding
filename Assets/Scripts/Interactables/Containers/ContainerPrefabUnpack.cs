using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>Unpacks a prefab when it is spawned (i.e., detaches the children and deletes itself).</summary>
public class ContainerPrefabUnpack : MonoBehaviour
{
    protected void Awake()
    {
        //Make sure that all children have the appropriate scripts assigned
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (!child)
            {
                continue;
            }

            CustomInteractable customInteractableScript = child.GetComponent<CustomInteractable>();
            if (!customInteractableScript)
            {
                child.AddComponent<CustomInteractable>();
            }

            HighlightInteractableObject highlightInteractableObjectScript = child.GetComponent<HighlightInteractableObject>();
            if (!highlightInteractableObjectScript)
            {
                child.AddComponent<HighlightInteractableObject>();
            }
        }
    }

    protected void Start()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
