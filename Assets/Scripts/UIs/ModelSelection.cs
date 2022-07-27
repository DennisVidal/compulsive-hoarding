using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelSelection : MonoBehaviour
{
    

    SortedList<int, SpawnableExtended> selectedSpawnables = new SortedList<int, SpawnableExtended>();
    int currentID = 0;
    GameObject currentGO;

    public RawImage checkMarkImage;

    public InputField spawnAmountIF;

    // Start is called before the first frame update
    void Start()
    {
        KeyValuePair<Object,int>[] spawnables = GameEvents.Instance.spawnables;

        for(int i = 0; i < spawnables.Length; i++)
        {
            SpawnableExtended newSpawnable = new SpawnableExtended();
            newSpawnable.spawnable = spawnables[i].Key;
            newSpawnable.id = i;
            newSpawnable.spawnAmount = spawnables[i].Value;
            selectedSpawnables.Add(i, newSpawnable);
        }

        UpdateCurrentModel();
    }

    void UpdateCurrentModel()
    {
        Destroy(currentGO);
        currentGO = Instantiate(selectedSpawnables[currentID].spawnable as GameObject, gameObject.transform, false);

        currentGO.layer = LayerMask.NameToLayer("ModelViewer");
        currentGO.AddComponent<ModelSpin>();

        checkMarkImage.enabled = selectedSpawnables[currentID].shouldSpawn;

        //Update Input Field
        HandleUpdateOfAmount();

    }

    void HandleUpdateOfAmount()
    {
        int amount = selectedSpawnables[currentID].spawnAmount;

        if(amount < 0)
        {
            selectedSpawnables[currentID].spawnAmount = 0;
        }
        else if (amount == 10001)
        {
            selectedSpawnables[currentID].spawnAmount = 1;
        }
        else if(amount > 9999)
        {
            selectedSpawnables[currentID].spawnAmount = 10000;
            spawnAmountIF.text = "\u221E";
            return;
        }


        spawnAmountIF.text = selectedSpawnables[currentID].spawnAmount.ToString();


    }

    public void ButtonPressNext()
    {
        currentID++;
        if(currentID >= selectedSpawnables.Count)
        {
            currentID = 0;
        }
        UpdateCurrentModel();
    }

    public void ButtonPressPrev()
    {
        currentID--;
        if (currentID < 0)
        {
            currentID = selectedSpawnables.Count-1;
        }
        UpdateCurrentModel();
    }

    public void ButtonPressBack()
    {
        List<KeyValuePair<Object,int>> newSpawnablesList = new List<KeyValuePair<Object, int>>();

        foreach  (KeyValuePair<int, SpawnableExtended> item in selectedSpawnables)
        {
            if (item.Value.shouldSpawn)
            {
                newSpawnablesList.Add(new KeyValuePair<Object, int>(item.Value.spawnable, item.Value.spawnAmount));
            }
        }

        GameEvents.Instance.spawnables = newSpawnablesList.ToArray();
        GameEvents.Instance.ButtonPressSpawnSettings();
        
    }

    public void InputAmountFinished()
    {
        selectedSpawnables[currentID].spawnAmount = int.Parse(spawnAmountIF.text);

        HandleUpdateOfAmount();
    }   

    public void RTPress()
    {
        selectedSpawnables[currentID].shouldSpawn = !selectedSpawnables[currentID].shouldSpawn;
        checkMarkImage.enabled = selectedSpawnables[currentID].shouldSpawn;
    }

    public void spawnAmountButtonPressed(int change)
    {
        int amount = selectedSpawnables[currentID].spawnAmount += change;
        HandleUpdateOfAmount();
    }
}

public class SpawnableExtended
{
    public Object spawnable;
    public int id;
    public bool shouldSpawn = true;
    public int spawnAmount;
}
