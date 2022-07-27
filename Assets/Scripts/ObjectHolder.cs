using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ObjectHolder : MonoBehaviour
{
    public int numberOfSpawnedObjects = 0;
    public float area;
    public float ratio;
    public int failedSpawns = 0;

    GameObject prefab;
    GameObject parentGO = null;

    Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        //Get parent gameobject
        parentGO = GameObject.Find("Spawnables");

        if (GetComponent<MeshCollider>() == null && GetComponent<BoxCollider>() == null)
        {
            Debug.LogWarning("ObjectHolderScript attached to a gameobject (" + this.gameObject.name + ") without a collider. Adding a mesh collider");
            this.gameObject.AddComponent<MeshCollider>();
        }

        //Calculate Area
        bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        area = bounds.size.x * bounds.size.y;
        UpdateRatio();

        //Register Script at Game Manager
        GameEvents.Instance.RegisterObjectHolder(this);
    }

    public bool SpawnObject(Object objectToSpawn)
    {
        Vector3 spawnVertex = Vector3.zero;
        Vector3 worldUp = transform.InverseTransformDirection(Vector3.up).normalized;

        //get random asset to spawn
        prefab = objectToSpawn as GameObject;

        //get bounds to calculate the place needed for the object
        Vector3 spawnableBounds = prefab.GetComponent<MeshRenderer>().bounds.extents;

        //TODO: check for rotation changes, atm we can only spawn on objects with at least 1 surface pointing directly to the top (transform rotation in 90 degree steps)
        //if (worldUp.x == 1)
        //{
        //    width = bounds.center.y + bounds.extents.y - spawnableBounds.x;
        //    length = bounds.center.z + bounds.extents.z - spawnableBounds.z;
        //    spawnVertex = new Vector3(bounds.center.x + bounds.extents.x, Random.Range(-width, width), Random.Range(-length, length));
        //}
        //else if (worldUp.y == 1)
        //{
        float width = bounds.center.x + bounds.extents.x - spawnableBounds.x;
        float length = bounds.center.z + bounds.extents.z - spawnableBounds.z;
        float width2 = bounds.center.x - bounds.extents.x + spawnableBounds.x;
        float length2 = bounds.center.z - bounds.extents.z + spawnableBounds.z;
        spawnVertex = new Vector3(Random.Range(width, width2), bounds.center.y + bounds.extents.y, Random.Range(length, length2));

        //}
        //else
        //{
        //    width = bounds.center.x + bounds.extents.x - spawnableBounds.x;
        //    length = bounds.center.y + bounds.extents.y - spawnableBounds.z;
        //    spawnVertex = new Vector3(Random.Range(-width, width), Random.Range(-length, length), bounds.center.z + bounds.extents.z);
        //}

        //check if object fits on parent
        if (bounds.extents.x < spawnableBounds.x || bounds.extents.z < spawnableBounds.z)
        {
            Debug.Log("No Place to spawn Object on  " + this.gameObject.name + ". No object will be spawned");
            IncerementFailed();
            return false;
        }
        //transform to world position
        spawnVertex = transform.TransformPoint(spawnVertex);

        //offset spawn position by a little bit to prevent initial colissions
        spawnVertex.y += 0.2f;

        //check if spawning inside another gameobject

        if (Physics.CheckBox(spawnVertex, spawnableBounds))
        {
            Debug.Log("Could not spawn object cause it would spawn inside another object");
            IncerementFailed();
            return false;
        }

        ////check if object will fall on parent object
        //RaycastHit hit;
        //Vector3 origin = spawnVertex;

        //origin.x += spawnableBounds.x - 0.01f;
        //origin.z += spawnableBounds.z - 0.01f;
        //Physics.Raycast(origin, Vector3.down, out hit, 500);

        //if (hit.transform.gameObject != this.gameObject)
        //{
        //    Debug.Log("Could not spawn object cause it would not spawn on the parent object");
        //    IncerementFailed();
        //    return;
        //}
        //origin.x += spawnableBounds.x - 0.01f;
        //origin.z -= spawnableBounds.z - 0.01f;
        //Physics.Raycast(origin, Vector3.down, out hit, 500);

        //if (hit.transform.gameObject != this.gameObject)
        //{
        //    Debug.Log("Could not spawn object cause it would not spawn on the parent object");
        //    IncerementFailed();
        //    return;
        //}

        //origin.x -= spawnableBounds.x - 0.01f;
        //origin.z += spawnableBounds.z - 0.01f;
        //Physics.Raycast(origin, Vector3.down, out hit, 500);

        //if (hit.transform.gameObject != this.gameObject)
        //{
        //    Debug.Log("Could not spawn object cause it would not spawn on the parent object");
        //    IncerementFailed();
        //    return;
        //}

        //origin.x -= spawnableBounds.x - 0.01f;
        //origin.z -= spawnableBounds.z - 0.01f;
        //Physics.Raycast(origin, Vector3.down, out hit, 500);
        //if (hit.transform.gameObject != this.gameObject)
        //{
        //    Debug.Log("Could not spawn object cause it would not spawn on the parent object");
        //    IncerementFailed();
        //    return;
        //}


        //spawn the prefab and place it
        GameObject spawnedObject = Instantiate(prefab, spawnVertex, Quaternion.identity, parentGO.transform);

        //spawnedObject.transform.parent = parentGO.transform;


        //register gameobject at GameManager
        numberOfSpawnedObjects++;
        //is already in tmp wegthrowable
        //GameEvents.Instance.RegisterTrash(spawnedObject);

        //add objectholder to object
        spawnedObject.AddComponent<ObjectHolder>();
        spawnedObject.AddComponent<CustomInteractable>();
        spawnedObject.AddComponent<HighlightInteractableObject>();
        UpdateRatio();
        return true;
    }

    void UpdateRatio()
    {
        if (numberOfSpawnedObjects > 0)
        {
            ratio = area / numberOfSpawnedObjects;
        }
        else
        {
            ratio = area;
        }
    }

    void IncerementFailed()
    {
        failedSpawns++;
        //Cancel Object spawning on too many fails
        if(failedSpawns > 50)
        {
            ratio = 0;
        }
    }

    public void Reset()
    {
        numberOfSpawnedObjects = 0;
        failedSpawns = 0;
        UpdateRatio();
    }
}
