using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


/// <summary>Script that is attached to a door.</summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class Door_Script : MonoBehaviour
{

    [Header("Should be assigned by hand.")]
    [Tooltip("The object the player actually grabs.")]
    public GameObject dummyDoorHandle;
    [Tooltip("The visible door handle.")]
    public GameObject actualDoorHandle;
    [Tooltip("Collisions with these colliders will be ignored by the door.")]
    public List<Collider> collidersToIgnore;
    [Tooltip("Script for highlighting on hand hover.")]
    public HighlightInteractableObject highlightScript;

    [Header("Door Settings")]
    [Tooltip("Rotation the handle receives when it is being grabbed by the player.")]
    public Vector3 handleRotationOnGrab;
    [Tooltip("Position of the hinge joint of the door.")]
    public Vector3 hingeJointPosition = Vector3.zero;
    [Tooltip("Rotation axis of the hinge joint of the door.")]
    public Vector3 hingeJointAxis = Vector3.up;
    [Tooltip("Should the hinge joint of the door limit its rotation?")]
    public bool hingeJointUseLimits = true;
    [Tooltip("Min Angle of the hinge joint if it uses limits, use negative values or zero.")]
    public float hingeJointAngleMin = 0.0f;
    [Tooltip("Max Angle of the hinge joint if it uses limits, use positive values or zero.")]
    public float hingeJointAngleMax = 90.0f;

    [Header("SoundSettings")]
    public AudioSource audioSource;
    public AudioClip[] lockedSounds;
    public AudioClip[] unlockedSounds;

    protected JointLimits hingeJointLimits;
    protected Rigidbody ownRigidBody;
    protected Collider ownCollider;
    protected HingeJoint ownHingeJoint;
    protected FixedJoint fixedJointOnHandle;
    protected Interactable handleInteractable;
    protected Door_Handle_Script doorHandleScript;

    protected bool isLocked;
    protected bool isMasterLocked;
    protected bool isHandleBeingGrabbed;

    void Awake()
    {
        if(!highlightScript)
        {
            highlightScript = GetComponent<HighlightInteractableObject>();
        }

        audioSource = GetComponent<AudioSource>();

        isLocked = true;
        isMasterLocked = false;
        isHandleBeingGrabbed = false;

        //Setup door 
        ownCollider = GetComponent<Collider>();

        if (collidersToIgnore.Count > 0)
        {
            for (int i = 0; i < collidersToIgnore.Count; i++)
            {
                Physics.IgnoreCollision(ownCollider, collidersToIgnore[i], true);
            }
        }

        ownRigidBody = Util.FindOrAddComponent<Rigidbody>(this.gameObject);
        ownRigidBody.useGravity = false;

        ownHingeJoint = Util.FindOrAddComponent<HingeJoint>(this.gameObject);
        ownHingeJoint.anchor = hingeJointPosition;
        ownHingeJoint.axis = hingeJointAxis;
        ownHingeJoint.useLimits = hingeJointUseLimits;

        if (ownHingeJoint.useLimits)
        {
            hingeJointLimits.min = hingeJointAngleMin;
            hingeJointLimits.max = hingeJointAngleMax;
            ownHingeJoint.limits = hingeJointLimits;
        }

        //Setup dummy door handle
        if (dummyDoorHandle)
        {
            if (!handleInteractable)
            {
                handleInteractable = Util.FindOrAddComponent<Interactable>(dummyDoorHandle);
            }

            if (!fixedJointOnHandle)
            {
                fixedJointOnHandle = Util.FindOrAddComponent<FixedJoint>(dummyDoorHandle);
            }

            ConnectDummyDoorHandle();
            //stabilises the door when door handle snaps back into position
            fixedJointOnHandle.massScale = 100;

            if (!doorHandleScript)
            {
                doorHandleScript = Util.FindOrAddComponent<Door_Handle_Script>(dummyDoorHandle);
            }

            doorHandleScript.actualDoorhandle = actualDoorHandle;
            doorHandleScript.handleRotation = handleRotationOnGrab;
            doorHandleScript.doorRigidbody = ownRigidBody;
            doorHandleScript.doorScript = this;

            //Ignore all collisions between the dummy door handle and the collider of the door itself 
            Physics.IgnoreCollision(ownCollider, dummyDoorHandle.GetComponent<Collider>(), true);
        }

        //Lock the door initially 
        LockDoor();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        //If the hand grabs the door handle, disable collision between the hand and the collider of the door 
        string name = collision.gameObject.name;
        if (name.Contains("HandCollider"))
        {
            if(isHandleBeingGrabbed)
            {
                Physics.IgnoreCollision(ownCollider, collision.collider, true);
            }
            else
            {
                Physics.IgnoreCollision(ownCollider, collision.collider, false);
            }
        }
    }

    /// <summary>Checks the current angle of the door.</summary>
    /// <returns>Current angle the door is at.</returns>
    public float GetCurrentAngle()
    {
        return ownHingeJoint.angle;
    }

    /// <summary>Locks the door.</summary>
    public void LockDoor()
    {
        ownRigidBody.isKinematic = true;
        isLocked = true;
    }

    /// <summary>Unlocks the door.</summary>
    public void UnlockDoor()
    {
        if(isMasterLocked)
        {
            //Debug.Log("Trying to unlock a door but it's master locked!");
            isLocked = true;

            if(!audioSource.isPlaying)
            {
                audioSource.clip = lockedSounds[Random.Range(0, lockedSounds.Length)];
                audioSource.PlayOneShot(audioSource.clip);
            }

            return;
        }

        ownRigidBody.isKinematic = false;
        isLocked = false;

        audioSource.clip = unlockedSounds[Random.Range(0, unlockedSounds.Length)];
        audioSource.PlayOneShot(audioSource.clip);
    }

    /// <summary>Masterlocks the door, which means that it can not be opened by the player.</summary>
    /// <param name="value">True or False.</param>
    public void SetMasterLock(bool value)
    {
        isMasterLocked = value;
    }

    /// <summary>Is the door currently masterlocked?.</summary>
    /// <returns>True if masterlocked, False if not.</returns>
    public bool IsMasterLocked()
    {
        return isMasterLocked;
    }

    /// <summary>Forces the door to close.</summary>
    public void ForceCloseDoor()
    {
        hingeJointLimits.min = -1.0f;
        hingeJointLimits.max = 1.0f;
        ownHingeJoint.limits = hingeJointLimits;

        StartCoroutine("ResetLimits");
    }

    /// <summary>Coroutine to reset the intended limits of the hinge joint a bit delayed (by one second).</summary>
    IEnumerator ResetLimits()
    {
        yield return new WaitForSeconds(1.0f);

        if (ownHingeJoint.useLimits)
        {
            hingeJointLimits.min = hingeJointAngleMin;
            hingeJointLimits.max = hingeJointAngleMax;
            ownHingeJoint.limits = hingeJointLimits;
        }
    }

    /// <summary>Set if the handle is currently being grabbed by a hand.</summary>
    public void SetIsHandleBeingGrabbed(bool value)
    {
        isHandleBeingGrabbed = value;
    }

    /// <summary>Is the handle currently being grabbed?</summary>
    /// <returns>True if the handle is being grabbed, False if not.</returns>
    public bool IsHandleBeingGrabbed()
    {
        return isHandleBeingGrabbed;
    }

    /// <summary>Is the door currently locked?</summary>
    /// <returns>True if the door is locked, False if not.</returns>
    public bool IsLocked()
    {
        return isLocked;
    }

    /// <summary>Disconnects the dummy door handle from the fixed joint.</summary>
    public void DisconnectDummyDoorHandle()
    {
        if (fixedJointOnHandle)
        {
            fixedJointOnHandle.connectedBody = null;
        }
    }

    /// <summary>Connects the dummy door handle to the fixed joint.</summary>
    public void ConnectDummyDoorHandle()
    {
        if(fixedJointOnHandle)
        {
            fixedJointOnHandle.connectedBody = ownRigidBody;
        }          
    }

    /// <summary>Enables the highlighting on the door.</summary>
    public void StartHighlightingDoor()
    {
        if(highlightScript)
        {
            highlightScript.HighlightOn();
        }
    }

    /// <summary>Disables the highlighting on the door.</summary>
    public void StopHighlightingDoor()
    {
        if (highlightScript)
        {
            highlightScript.HighlightOff();
        }
    }
}