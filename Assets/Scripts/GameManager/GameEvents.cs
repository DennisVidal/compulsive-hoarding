using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO;

public class GameEvents : MonoBehaviour
{
    //Variables
    public List<GameObject> trashList;

    public Door_Script door_Script;

    public Player vrPlayer;
    private Vector3 vrPlayerInitPos;

    //holds the trashvalue to make it accessible for steam-vr
    public ValueHolder valueHolderStemvr;

    //Cams
    public GameObject SpectCam;
    public GameObject VrCam;

    private bool hasBeenRead = false;
    private int maxTrash = 5;
    private int thrownAway = 0;
    float trashRatio = 1;

    [Range(0, 10)] public float lightAnimationDuration = 5.0f;

    //Spawning
    public KeyValuePair<UnityEngine.Object, int>[] spawnables;
    public KeyValuePair<UnityEngine.Object, int>[] spawnablesBeforeSpawn;

    public List<ObjectHolder> objectHolderList;
    public List<ObjectHolder> initialObjectHolder;
    public float timeBetweenSpawns = 0.5f;
    public int spawnAmount = 0;

    //UIs
    public GameObject currentUI;
    public GameObject InterfaceCanvas;
    public GameObject SelectionUI;
    public GameObject CreditsUI;

    [Tooltip("List of containers in the scene that can spawn objects, containers add themselves automatically on startup.")]
    public List<Container> containerList;
    [Tooltip("List of prefabs that can be spawned in the containers.")]
    public UnityEngine.Object[] containerSpawnables;

    [Tooltip("Percentage of containers to fill with objects.")]
    [Range(0, 100)]
    public int containerPercentageToFill = 50;

    //Events
    public event Action<float> onTrashCountChange;


    //All the Single(ton) Ladys!
    public static GameEvents Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

    }

    private void Start()
    {
        //Get ValueHolder for Steamvr
        valueHolderStemvr = FindObjectOfType<ValueHolder>();

        //Load prefabs to spawn
        UnityEngine.Object[] tmpspawnables = Resources.LoadAll("Spawnables");
        List<KeyValuePair<UnityEngine.Object, int>> checkedObjects = new List<KeyValuePair<UnityEngine.Object, int>>();

        //check for bounding box so collision detection works
        bool canBeAdded = false;
        for (int i = 0; i < tmpspawnables.Length; i++)
        {
            GameObject currentGameobject = tmpspawnables[i] as GameObject;
            if (currentGameobject.GetComponent<BoxCollider>() != null) canBeAdded = true;
            if (currentGameobject.GetComponent<MeshCollider>() != null)
            {
                if (currentGameobject.GetComponent<MeshCollider>().convex) canBeAdded = true;
            }
            if (canBeAdded)
            {
                checkedObjects.Add(new KeyValuePair<UnityEngine.Object, int>(tmpspawnables[i], 10000));
            }
        }
        spawnables = checkedObjects.ToArray();

        //load data from preset amounts
        string path = "Assets/Prefabs/Objects/Resources/presets.txt";
        StreamReader reader = new StreamReader(path);
        string reading;
        List<int> inputNumber = new List<int>();
        do
        {
            reading = reader.ReadLine();

            int tmpInt;
            if (int.TryParse(reading, out tmpInt))
            {
                inputNumber.Add(tmpInt);
            }


        } while (reading != null);
        reader.Close();

        if (inputNumber.Count == spawnables.Length)
        {
            List<KeyValuePair<UnityEngine.Object, int>> keyValuePairs = new List<KeyValuePair<UnityEngine.Object, int>>();
            for (int i = 0; i < inputNumber.Count; i++)
            {
                keyValuePairs.Add(new KeyValuePair<UnityEngine.Object, int>(spawnables[i].Key, inputNumber[i]));
            }
            spawnables = keyValuePairs.ToArray();
        }




            //init playerposition
            vrPlayerInitPos = vrPlayer.rigSteamVR.transform.position;

        //Master lock the courtyard door on startup
        if (door_Script)
        {
            door_Script.SetMasterLock(true);
        }

        //Load all prefabs that can spawn in containers on startup
        containerSpawnables = Resources.LoadAll("ContainerSpawnables");

        //UI
        currentUI = InterfaceCanvas;
    }

    //public Methods
    public void TrashCountChange(float state)
    {
        if (onTrashCountChange != null)
        {
            onTrashCountChange(state);
        }
    }

    public void RegisterTrash(GameObject _trashObject)
    {
        trashList.Add(_trashObject);
        //Debug.Log("listsize = " + trashList.Count);
        if (hasBeenRead)
        {
            thrownAway--;
            RecalculateTrashRatio();
        }

    }

    public void UnregisterTrash(GameObject _trashObject)
    {
        if (!hasBeenRead)
        {
            hasBeenRead = true;
            //maxTrash = trashList.Count;
            //Debug.Log("maxtrash " + maxTrash);
        }
        thrownAway++;
        trashList.Remove(_trashObject);
        //Debug.Log("listsize = " + trashList.Count);
        RecalculateTrashRatio();
    }


    public void RegisterObjectHolder(ObjectHolder _objectHolder)
    {
        objectHolderList.Add(_objectHolder);
    }
    public IEnumerator Spawn()
    {
        //first spawn, save initial objectHolder
        if (initialObjectHolder.Count == 0)
        {
            foreach (ObjectHolder item in objectHolderList)
            {
                initialObjectHolder.Add(item);
            }
        }

        if (objectHolderList.Count == 0)
        {
            Debug.LogWarning("No Gameobject with ObjectHolder script found. Aborting spawning");
            yield break;
        }

        //Timer between spawns for physics to work
        while (true)
        {
            if (trashList.Count >= spawnAmount)
            {
                Debug.LogWarning("Spawning finished. Spawned " + trashList.Count + " of " + spawnAmount + ".");
                ResetSpawnables();
                yield break;
            }

            //spawn object on holder with the least spawnedobjects/area ratio
            ObjectHolder currentHolder = objectHolderList[0];

            foreach (ObjectHolder item in objectHolderList)
            {
                if (currentHolder.ratio < item.ratio)
                {
                    currentHolder = item;
                }
            }

            if (currentHolder.ratio != 0)
            {
                int rnd;
                KeyValuePair<UnityEngine.Object, int> kvp;
                bool foundObject = false;
                do
                {
                    if (spawnables.Length == 0)
                    {
                        Debug.LogWarning("Spawned the number of selected Objects .Spawned " + trashList.Count + " of " + spawnAmount + ".");
                        ResetSpawnables();
                        yield break;
                    }

                    rnd = UnityEngine.Random.Range(0, spawnables.Length);
                    kvp = spawnables[rnd];



                    if(kvp.Value == 0)
                    {
                        for (int i = rnd; i < spawnables.Length-1; i++)
                        {
                            spawnables[i] = spawnables[i + 1];
                        }
                        Array.Resize(ref spawnables, spawnables.Length - 1);
                    }
                    else
                    {
                        foundObject = true;
                    }
                    

                } while (!foundObject);
                

                if (currentHolder.SpawnObject(kvp.Key))
                {
                    spawnables[rnd] = new KeyValuePair<UnityEngine.Object, int>(kvp.Key, kvp.Value - 1);
                }
            }
            else
            {
                Debug.LogWarning("No more spawning possible. Spawned " + trashList.Count + " of " + spawnAmount + ".");
                ResetSpawnables();
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    //private Methods
    private void RecalculateTrashRatio()
    {
        //trashRatio = trashList.Count / (float)maxTrash;

        trashRatio = (maxTrash - thrownAway) / (float)maxTrash;
        Mathf.Clamp(trashRatio, 0, 1);
        TrashCountChange(trashRatio);
        valueHolderStemvr.SetTrashValue(trashRatio);
    }

    private void DeleteTrash()
    {
        foreach (ObjectHolder holder in objectHolderList)
        {
            holder.Reset();
        }
        foreach (GameObject trash in trashList)
        {
            if (!trash.CompareTag("PrePlaced"))
            {
                Destroy(trash);
            }
        }
        trashList.Clear();

        objectHolderList.Clear();
        foreach (ObjectHolder item in initialObjectHolder)
        {
            objectHolderList.Add(item);
        }

    }

    //Button Handling
    public void ButtonPressStartTherapy(int maxClearTrash)
    {
        // Unlock Door asdf
        door_Script.SetMasterLock(false);
        // Adjust Lighting
        maxTrash = maxClearTrash;
    }

    public void ButtonPressStopTherapy()
    {
        // Reset UMD to Yard 
        vrPlayer.rigSteamVR.transform.position = vrPlayerInitPos;

        // Lock door
        door_Script.ForceCloseDoor();
        door_Script.SetMasterLock(true);

        // Delete Trash
        DeleteTrash();
    }

    public void ButtonPressSpawn(int objectNumber)
    {
        //delete old objects
        if (initialObjectHolder.Count != 0)
        {
            DeleteTrash();
        }

        //Spawn Objects
        spawnAmount = objectNumber + trashList.Count;

        spawnablesBeforeSpawn = new KeyValuePair<UnityEngine.Object, int>[spawnables.Length];
        System.Array.Copy(spawnables, spawnablesBeforeSpawn, spawnables.Length);

        StartCoroutine(Spawn());

        //Spawn objects in containers
        StartCoroutine(FillContainers());
    }

    public void ResetSpawnables()
    {
        spawnables = spawnablesBeforeSpawn;
    }


    public void ButtonPressEndGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void ButtonPressSpawnSettings()
    {
        //Switch UI
        if(currentUI == InterfaceCanvas)
        {
            currentUI.SetActive(false);
            currentUI = SelectionUI;
            currentUI.SetActive(true);

        }
        else
        {
            currentUI.SetActive(false);
            currentUI = InterfaceCanvas;
            currentUI.SetActive(true);
        }

    }

    public void ButtonPressOpenCredits()
    {
        //Switch UI
        if (currentUI == InterfaceCanvas)
        {
            currentUI.SetActive(false);
            currentUI = CreditsUI;
            currentUI.SetActive(true);

        }
        else
        {
            currentUI.SetActive(false);
            currentUI = InterfaceCanvas;
            currentUI.SetActive(true);
        }
    }





    public GameObject getVRCam()
    {
        return this.VrCam;
    }

    public GameObject getSpectatorCam()
    {
        return this.SpectCam;
    }


    public void RegisterContainer(Container container)
    {
        containerList.Add(container);
    }

    public void UnregisterContainer(Container container)
    {
        containerList.Remove(container);
    }

    /// <summary>Tries to spawn random objects in the specific container.</summary>
    /// <param name="index">Index of the container.</param>
    /// <returns>True if any objects were able to be spawned, False if not.</returns>
    public bool SpawnObjectInContainer(int index)
    {
        if (containerSpawnables.Length == 0 || containerList.Count == 0)
        {
            return false;
        }

        Container container = containerList[index];

        //Don't spawn objects if there are already objects inside
        if (container.HasObjectsInside())
        {
            return false;
        }

        if (container.spawnLocations.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < container.spawnLocations.Length; i++)
        {
            GameObject prefabToSpawn = ChoosePrefabForContainer();
            GameObject spawnedObject = Instantiate(prefabToSpawn, container.spawnLocations[i].position, container.spawnLocations[i].rotation);

            for (int j = 0; j < spawnedObject.transform.childCount; j++)
            {
                CustomInteractable customInteractable = spawnedObject.transform.GetChild(j).gameObject.GetComponent<CustomInteractable>();
                if (!customInteractable)
                {
                    customInteractable = spawnedObject.transform.GetChild(j).gameObject.AddComponent<CustomInteractable>();
                }
            }
        }

        return true;
    }

    /// <summary>Fills containers with objects.</summary>
    IEnumerator FillContainers()
    {
        //If there are no containers registered, don't do anything
        int containerCount = containerList.Count;
        if (containerCount == 0)
        {
            yield break;
        }

        int numberOfContainersToFill = Mathf.FloorToInt(containerPercentageToFill * 0.01f * containerCount);

        if (numberOfContainersToFill == 0)
        {
            if(containerPercentageToFill > 0)
            {
                numberOfContainersToFill = 1;
            }
            else
            {
                yield break;
            }
        }

        //Choose random containers to fill, depending on the percentage of containers that have to be filled
        List<int> indices = new List<int>();
        for (int i = 0; i < containerCount; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < numberOfContainersToFill; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, indices.Count);
            int containerIndex = indices[randomIndex];
            indices.RemoveAt(randomIndex);

            SpawnObjectInContainer(containerIndex);

            //Wait one frame to update container states
            yield return null;
        }
    }

    //TODO: Adjust this if specific prefabs have to be spawned instead of just random ones
    /// <summary>Chooses a specific prefab to spawn in a container.</summary>
    /// <returns>GameObject to spawn.</returns>
    protected GameObject ChoosePrefabForContainer()
    {
        return containerSpawnables[UnityEngine.Random.Range(0, containerSpawnables.Length)] as GameObject;
    }
}
