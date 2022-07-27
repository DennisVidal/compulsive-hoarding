using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Checks for trigger collisions and reports to the assigned container.</summary>
public class ContainerCollisionChecker : MonoBehaviour
{
    public enum CollisionCheckerType
    {
        Ground, 
        Front, 
        Top
    }

    [Tooltip("Container this collision checker is assigned to.")]
    public Container container;
    [Tooltip("What type of collision checker is this?")]
    public CollisionCheckerType type; 

    protected void Awake()
    {
        if(!container)
        {
            container = transform.parent.GetComponent<Container>();
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(!container)
        {
            return;
        }

        CustomInteractable customInteractable = other.gameObject.GetComponent<CustomInteractable>();
        if (customInteractable)
        {
            if(type == CollisionCheckerType.Ground)
            {
                if(customInteractable.isBeingGrabbed())
                {
                    return; 
                }

                container.AddPossibleObject(customInteractable);
            }
            else if(type == CollisionCheckerType.Front)
            {
                int index = container.GetContainedObjectIndex(customInteractable);
                if (index != -1)
                {
                    container.SetIsCollidingWithFront(index, true);
                }
            }
            else if(type == CollisionCheckerType.Top)
            {
                int index = container.GetContainedObjectIndex(customInteractable);
                if (index != -1)
                {
                    container.SetIsCollidingWithTop(index, true);
                }
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!container)
        {
            return;
        }

        CustomInteractable customInteractable = other.gameObject.GetComponent<CustomInteractable>();
        if (customInteractable)
        {
            if (type == CollisionCheckerType.Ground)
            {
                container.RemovePossibleObject(customInteractable);
                container.RemoveObject(customInteractable);
            }
            else if (type == CollisionCheckerType.Front)
            {
                int index = container.GetContainedObjectIndex(customInteractable);
                if (index != -1)
                {
                    container.SetIsCollidingWithFront(index, false);
                }
            }
            else if (type == CollisionCheckerType.Top)
            {
                int index = container.GetContainedObjectIndex(customInteractable);
                if (index != -1)
                {
                    container.SetIsCollidingWithTop(index, false);
                }
            }
        }
    }
}
