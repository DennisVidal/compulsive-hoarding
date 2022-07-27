using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CustomInteractable : Throwable
{
    [Tooltip("Is the item currently in a container?")]
    public bool isInContainer;

    void Start()
    {
        GameEvents.Instance.RegisterTrash(this.gameObject);
    }

    /// <summary>Is the object being grabbed by a hand?</summary>
    /// <returns>True if is being grabbed, False if not.</returns>
    public bool isBeingGrabbed()
    {
        return attached;
    }

    private void OnTriggerEnter(Collider other)
    {
        GarbageCanObjectCounter ccc;

        if (other.TryGetComponent<GarbageCanObjectCounter>(out ccc))
        {
            GameEvents.Instance.UnregisterTrash(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GarbageCanObjectCounter ccc;

        if (other.TryGetComponent<GarbageCanObjectCounter>(out ccc))
        {
            GameEvents.Instance.RegisterTrash(this.gameObject);
        }
    }

    /*
    //Code to check if physics are still needed or not if an object is inside a container
    //Not really needed since rigidbodies automatically stop calculating collisions when they are not being moved for a short period of time 

    protected Vector3 positionOneFrameAgo;
    protected Vector3 positionTwoFramesAgo;
    public bool needsPhysics = true;

    protected void CheckPhysics()
    {
        bool oldNeedForPhysics = needsPhysics;
        CheckNeedForPhysics();

        if(oldNeedForPhysics == needsPhysics)
        {
            return;
        }
       
        if(needsPhysics)
        {
            ownRigidbody.isKinematic = false;
        }
        else
        {
            ownRigidbody.isKinematic = true;
        }
    }

    protected void CheckNeedForPhysics()
    { 
        if(isInContainer)
        {
            //If the position has not really changed in the last two frames then disable physics
            Vector3 currentPosition = transform.position;
            if(positionTwoFramesAgo != Vector3.zero && positionOneFrameAgo != Vector3.zero)
            {
                if(AreVectorsRoughlyEqual(currentPosition, positionOneFrameAgo) && AreVectorsRoughlyEqual(positionOneFrameAgo, positionTwoFramesAgo))
                {
                    needsPhysics = false;
                }
                else
                {
                    needsPhysics = true;
                }
            }

            positionTwoFramesAgo = positionOneFrameAgo;
            positionOneFrameAgo = transform.position;                
        }
    }

    protected bool AreVectorsRoughlyEqual(Vector3 a, Vector3 b)
    {
        float minDistance = 0.000001f;
        if(Vector3.SqrMagnitude(a - b) <= minDistance)
        {
            return true;
        }

        return false;
    }
    */
}
