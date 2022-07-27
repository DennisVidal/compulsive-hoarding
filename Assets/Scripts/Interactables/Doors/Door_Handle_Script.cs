using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//This script will be automatically added to the dummyDoorHandle of a door through the door script
//Do not add this manually
[RequireComponent(typeof(Rigidbody))]
public class Door_Handle_Script : MonoBehaviour
{
    [EnumFlags]
    public Hand.AttachmentFlags attachmentFlags = 0;

    [HideInInspector]
    public Rigidbody ownRigidbody;
    List<Hand> hands;

    Vector3 localPosAtStart;

    [HideInInspector]
    public float maxDistance = 0.3f;

    [HideInInspector]
    public Rigidbody doorRigidbody;
  
    bool isBeingGrabbed;

    [HideInInspector]
    public Interactable ownInteractable;

    [HideInInspector]
    public GameObject actualDoorhandle;

    [HideInInspector]
    public Vector3 handleRotation;

    Quaternion originalRotation;

    [HideInInspector]
    public Door_Script doorScript;

    void Awake()
    {
        hands = new List<Hand>();
    }
    void Start()
    {
        //if no rigidbody is attached, add one 
        ownRigidbody = Util.FindOrAddComponent<Rigidbody>(gameObject);

        localPosAtStart = gameObject.transform.localPosition;

        originalRotation = actualDoorhandle.transform.localRotation;

        if(!doorScript)
        {
            Debug.LogError("Missing a door script!");
        }
    }

    void Update()
    {       
        //check if saved hands stopped grabbing the object
        CheckForHandRelease();

        //check if the object is too far away from the door handle position
        CheckForDistanceContrain();

        if (doorScript.GetCurrentAngle() < 1.0f && doorScript.GetCurrentAngle() > -1.0f)
        {
            if (isBeingGrabbed && doorScript.IsLocked())
            {
                doorScript.UnlockDoor();
            }
            
            if(!isBeingGrabbed && !doorScript.IsLocked())
            {
                doorScript.LockDoor();
            }
        }
    }

    protected void CheckForDistanceContrain()
    {      
        if (Vector3.SqrMagnitude(localPosAtStart - transform.localPosition) >= maxDistance * maxDistance)
        {
            for (int i = 0; i < hands.Count; i++)
            {
                DetachWithPhysics(hands[i]);
            }
        }
    }

    protected void CheckForHandRelease()
    {
        for (int i = 0; i < hands.Count; i++)
        {
            if (hands[i].IsGrabEnding(gameObject))
            {
                DetachWithPhysics(hands[i]);
            }
        }
    }

    protected virtual void OnHandHoverBegin(Hand hand)
    {
        if(doorScript)
        {
            doorScript.StopHighlightingDoor();
            doorScript.StartHighlightingDoor();
        }
    }

  
    void HandHoverUpdate(Hand hand)
    {
        GrabTypes grabTypeOnStart = hand.GetGrabStarting();

        if (grabTypeOnStart != GrabTypes.None)
        {
            AttachWithPhysics(hand, grabTypeOnStart);
        }
    }

    protected virtual void OnHandHoverEnd(Hand hand)
    {
        if (doorScript)
        {
            doorScript.StopHighlightingDoor();
        }
    }

    void RotateDoorhandle()
    {
        actualDoorhandle.transform.localRotation = originalRotation * Quaternion.Euler(handleRotation);
    }

    void ResetDoorhandleRotation()
    {
        actualDoorhandle.transform.localRotation = originalRotation;
    }

    void AttachWithPhysics(Hand hand, GrabTypes grabTypeOnStart)
    {
        DetachWithPhysics(hand);

        if (doorScript)
        {
            doorScript.StopHighlightingDoor();
        }
        
        doorScript.DisconnectDummyDoorHandle();

        RotateDoorhandle();
        isBeingGrabbed = true;
        doorScript.SetIsHandleBeingGrabbed(true);

        //if door handle has no rigidbody do nothing
        if (ownRigidbody == null)
        {
            return;
        }

        Rigidbody handRigidbody = Util.FindOrAddComponent<Rigidbody>(hand.gameObject);
        handRigidbody.isKinematic = true;

        //create fixed joint for attachment to rigidbody of the door handle
        FixedJoint handFixedJoint = hand.gameObject.AddComponent<FixedJoint>();
        handFixedJoint.connectedBody = ownRigidbody;

        //hand can only interact with this
        hand.HoverLock(null);
        //door handle gets attached to hand
        hand.AttachObject(gameObject, grabTypeOnStart, attachmentFlags);

        Vector3 offsetOfHand = hand.transform.position - ownRigidbody.worldCenterOfMass;
        offsetOfHand = Mathf.Min(offsetOfHand.magnitude, 1.0f) * offsetOfHand.normalized;

        hands.Add(hand);

        doorScript.ConnectDummyDoorHandle();
    }

    void DetachWithPhysics(Hand hand)
    {
        int i = hands.IndexOf(hand);

        if (i == -1)
        {
            return;
        }

        ResetDoorhandleRotation();
        NotBeingGrabbed();

        hands[i].DetachObject(gameObject, false);
        hands[i].HoverUnlock(null);

        Destroy(hands[i].GetComponent<FixedJoint>());
        Util.FastRemove(hands, i);
    }

    void BeingGrabbed()
    {
        isBeingGrabbed = true;
        doorScript.SetIsHandleBeingGrabbed(true);
    }

    void NotBeingGrabbed()
    {
        isBeingGrabbed = false;
        doorScript.SetIsHandleBeingGrabbed(false);
    }
}