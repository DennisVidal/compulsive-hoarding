using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

///// <summary></summary>
///// <param name=""></param>
///// <returns></returns>

/// <summary>Saves the different attributes of objects that are contained inside a container.</summary>
public class ContainedObject
{
    public CustomInteractable interactable;
    public bool isCollidingWithTop;
    public bool isCollidingWithFront;
}

/*
public enum ContainerType
{
    LivingRoom, 
    DiningRoom,
    Kitchen
}
*/

/// <summary>Can store multiple objects and can be instructed to spawn a specific object inside.</summary>
public class Container : Interactable
{
    //Objects that are fully placed in the container
    protected List<ContainedObject> containedObjects;
    //Objects that might be added to the container
    protected List<CustomInteractable> possibleObjects;

    [Tooltip("Material that will be used if the container is blocking.")]
    public Material blockMaterial;
    protected Material originalMaterial;
    protected MeshRenderer meshRenderer;
    
    protected Interactable interactableScript;
    protected DrawerLinearDrive linearDriveScript;

    [Tooltip("Is closing of the container being blocked?")]
    protected bool isClosingBlocked;
    protected bool wasClosingBlocked;

    [Tooltip("Distance at which the hand will be forced to detach.")]
    public float maxGrabDistance = 0.5f;
    protected float initialGrabDistance;
    protected float currentGrabDistance;
    
    [Tooltip("List of prefabs that can be spawned in the container.")]
    public List<GameObject> spawnablePrefabs;
    [Tooltip("Locations at which the prefabs can be spawned.")]
    public Transform[] spawnLocations;

    override protected void Start()
    {
        base.Start();
        containedObjects = new List<ContainedObject>();
        possibleObjects = new List<CustomInteractable>();

        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        interactableScript = GetComponent<Interactable>();
        linearDriveScript = GetComponent<DrawerLinearDrive>();

        isClosingBlocked = false;
        wasClosingBlocked = false;
        RegisterContainerWithGameManager();
    }

    override protected void Update()
    {
        base.Update();
        AddPossibleObjectsToContainer();
        CheckForDistanceContrain();
    }

    protected void FixedUpdate()
    {
        CheckForCollisions();
    }

    override protected void OnDestroy()
    {    
        UnregisterContainerWithGameManager();
        base.OnDestroy();
    }

    override protected void OnAttachedToHand(Hand hand)
    {
        base.OnAttachedToHand(hand);
        initialGrabDistance = Vector3.SqrMagnitude(hand.transform.position - transform.position);
    }


    ///// <summary>Spawns an object prefab in the container.</summary>
    ///// <returns>True if spawning was successful, False if not.</returns>
    //public bool SpawnObject()
    //{
    //    if (containedObjects.Count != 0)
    //    {
    //        return false; 
    //    }

    //    //TODO Get spawnable prefabs from gamemanager
    //    //spawnablePrefabs = ....

    //    if(spawnablePrefabs.Count == 0)
    //    {
    //        return false; 
    //    }

    //    int randomIndex = Random.Range(0, spawnablePrefabs.Count);

    //    GameObject prefab = Instantiate(spawnablePrefabs[randomIndex], transform.position, transform.rotation);

        
    //    prefab.transform.DetachChildren();
    //    Destroy(prefab);

    //    return true; 
    //}

    /// <summary>Forces the attached hand to detach.</summary>
    public void ForceDetachHand()
    {
        Hand hand = interactableScript.attachedToHand;
        if(hand)
        {
            hand.DetachObject(gameObject);
        }
    }

    /// <summary>Checks if the hand is still close enough to the original grabbing position and disconnects it if it is too far away.</summary>
    protected void CheckForDistanceContrain()
    {
        if(!interactableScript.attachedToHand)
        {
            return;
        }

        currentGrabDistance = Vector3.SqrMagnitude(interactableScript.attachedToHand.transform.position - transform.position);
        float maxDistanceSqrt = (initialGrabDistance + maxGrabDistance) * (initialGrabDistance + maxGrabDistance);

        if (currentGrabDistance >= maxDistanceSqrt)
        {
            ForceDetachHand();
        }
    }

    /// <summary>Adds a potential object fully to the container.</summary>
    protected void AddPossibleObjectsToContainer()
    {
        for (int i = 0; i < possibleObjects.Count; i++)
        {
            if (!possibleObjects[i].isBeingGrabbed())
            {
                AddObject(possibleObjects[i]);
            }
        }
    }

    /// <summary>Checks if there is an object blocking the container from closing.</summary>
    protected void CheckForCollisions()
    {
        if(containedObjects.Count != 0)
        {
            for (int i = 0; i < containedObjects.Count; i++)
            {
                if (containedObjects[i].isCollidingWithFront && containedObjects[i].isCollidingWithTop)
                {
                    isClosingBlocked = true;
                    break;
                }

                isClosingBlocked = false;
            }
        }
        else
        {
            isClosingBlocked = false;
        }    

        if(isClosingBlocked == wasClosingBlocked)
        {
            return;
        }
        else
        {
            wasClosingBlocked = isClosingBlocked;
        }

        if(isClosingBlocked)
        {
            BlockClosing();
        }
        else
        {
            UnblockClosing();
        }
    }

    /// <summary>Blocks closing of the container (drawer).</summary>
    public void BlockClosing()
    {
        if (blockMaterial)
        {
            meshRenderer.material = blockMaterial;
        }

        linearDriveScript.maintainMomemntum = false;
        linearDriveScript.isInteractable = false;
        //ForceDetachHand();
    }

    /// <summary>Unblocks closing of the container (drawer).</summary>
    public void UnblockClosing()
    {
        if (meshRenderer.material != originalMaterial)
        {
            meshRenderer.material = originalMaterial;
        }

        linearDriveScript.maintainMomemntum = true;
        linearDriveScript.isInteractable = true;
    }

    /// <summary>Is a contained object colliding with the front trigger?</summary>
    /// <param name="index">Index of the contained object.</param>
    /// <param name="value">Is it colliding or not?</param>
    public void SetIsCollidingWithFront(int index, bool value)
    {
        containedObjects[index].isCollidingWithFront = value;
    }

    /// <summary>Is a contained object colliding with the top trigger?</summary>
    /// <param name="index">Index of the contained object.</param>
    /// <param name="value">Is it colliding or not?</param>
    public void SetIsCollidingWithTop(int index, bool value)
    {
        containedObjects[index].isCollidingWithTop = value;
    }


    /// <summary>Fully add an object to the container.</summary>
    /// <param name="obj">Object to add to the container.</param>
    public void AddObject(CustomInteractable obj)
    {
        if (!Contains(obj))
        {
            ContainedObject containedObject = new ContainedObject();
            containedObject.interactable = obj;
            containedObject.isCollidingWithFront = false;
            containedObject.isCollidingWithTop = false;
            containedObjects.Add(containedObject);

            obj.transform.parent = transform;
            obj.isInContainer = true;
            RemovePossibleObject(obj);

        }
    }

    /// <summary>Remove a fully added object from the container.</summary>
    /// <param name="obj">Object to remove from the container.</param>
    public void RemoveObject(CustomInteractable obj)
    {
        int index = GetContainedObjectIndex(obj);
        if(index != -1)
        {
            containedObjects.RemoveAt(index);
            obj.transform.parent = null;
            obj.isInContainer = false;
        }
    }

    /// <summary>Add a possible object to the container.</summary>
    /// <param name="obj">Possible object to add.</param>
    public void AddPossibleObject(CustomInteractable obj)
    {
        if(!possibleObjects.Contains(obj))
        {
            possibleObjects.Add(obj);
        }     
    }

    /// <summary>Remove a possible object from the container.</summary>
    /// <param name="obj">Possible object to remove.</param>
    public void RemovePossibleObject(CustomInteractable obj)
    {
        for (int i = 0; i < possibleObjects.Count; i++)
        {
            if(possibleObjects[i] == obj)
            {
                possibleObjects.RemoveAt(i);
            }
        }
    }

    /// <summary>Is the object already in the container?</summary>
    /// <param name="obj">Object to check for.</param>
    /// <returns>True if already in container, False if not.</returns>
    public bool Contains(CustomInteractable obj)
    {
        for (int i = 0; i < containedObjects.Count; i++)
        {
            if(containedObjects[i].interactable == obj)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets the index of a contained object.</summary>
    /// <param name="obj">Object to check for.</param>
    /// <returns>Index of the contained object, -1 if object is not in container.</returns>
    public int GetContainedObjectIndex(CustomInteractable obj)
    {
        int index = -1;

        for (int i = 0; i < containedObjects.Count; i++)
        {
            if(containedObjects[i].interactable == obj)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    /// <summary>Checks if there are any objects in the container.</summary>
    /// <returns>True if there is at least one object in the container, False if there is none.</returns>
    public bool HasObjectsInside()
    {
        if(containedObjects.Count != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>Register the container with the game manager for object spawning.</summary>
    public void RegisterContainerWithGameManager()
    {
        GameEvents.Instance.RegisterContainer(this);
    }

    /// <summary>Unregister the container from the game manager.</summary>
    public void UnregisterContainerWithGameManager()
    {
        GameEvents.Instance.UnregisterContainer(this);
    }
}
