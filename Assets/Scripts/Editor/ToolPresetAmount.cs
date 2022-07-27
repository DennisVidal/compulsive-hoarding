using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ToolPresetAmount : EditorWindow
{
    List<UnityEngine.GameObject> prefabs = new List<GameObject>();
    List<int> prefabAmount = new List<int>();
    List<Editor> gameObjectEditors = new List<Editor>();
    List<string> stringsToEdit = new List<string>();

    Vector2 scrollPos;

    [MenuItem("Tools/Preset Spawn Item Amount")]
    public static void ShowWinow()
    {
        GetWindow<ToolPresetAmount>("Preset Spawn Item Amount");
    }

    //GUI Main Window
    void OnGUI()
    {
        //if (GUILayout.Button("Update List"))
        //{
        //    UpdateList();
        //}
        if (GUILayout.Button("Load"))
        {
            Load();
        }
        if (GUILayout.Button("Save"))
        {
            Save();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (prefabs != null)
        {
            if (prefabs.Count != 0)
            {
                GUILayout.Label("Found Prefabs", EditorStyles.boldLabel);

                for (int i = 0; i < prefabs.Count; i++)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label(prefabs[i].name);
                    GUILayout.BeginHorizontal();
                    gameObjectEditors[i].OnPreviewGUI(GUILayoutUtility.GetRect(50, 50, 50, 50), EditorStyles.whiteLabel);
                    stringsToEdit[i] = GUILayout.TextField(stringsToEdit[i]);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();

                    int tmpInt;
                    if (int.TryParse(stringsToEdit[i], out tmpInt)){
                        prefabAmount[i] = tmpInt;
                    }
                }
            }

            else
            {
                GUILayout.Label("No Prefabs found", EditorStyles.boldLabel);
            }
        }
        else
        {
            GUILayout.Label("Failed to load Prefabs", EditorStyles.boldLabel);
        }
        EditorGUILayout.EndScrollView();
    }

    void UpdateList()
    {
        UnityEngine.Object[] tmpspawnables = Resources.LoadAll("Spawnables");
        prefabs.Clear();
        prefabAmount.Clear();
        stringsToEdit.Clear();
        foreach (var item in gameObjectEditors)
        {
            DestroyImmediate(item);
        }
        gameObjectEditors.Clear();

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
                prefabs.Add(currentGameobject);
                int amount = 0;
                prefabAmount.Add(amount);
                stringsToEdit.Add(amount.ToString());
                gameObjectEditors.Add(Editor.CreateEditor(currentGameobject));

                canBeAdded = false;
            }
        }
    }

    void Load()
    {
        UpdateList();
        if (File.Exists("Assets/Prefabs/Objects/Resources/presets.txt"))
        {
            StreamReader reader = new StreamReader("Assets/Prefabs/Objects/Resources/presets.txt");
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

            if (inputNumber.Count == prefabAmount.Count)
            {
                for (int i = 0; i < inputNumber.Count; i++)
                {
                    prefabAmount[i] = inputNumber[i];
                    stringsToEdit[i] = inputNumber[i].ToString();
                }
            }
            else
            {
                Debug.LogError("PresetAmount file has incompatible amount of prefabs saved, file can not be loaded");
            }
        }
        else
        {
            Debug.LogError("Missing preset file for spawnables!");
        }
    }
    void Save()
    {
        if (File.Exists("Assets/Prefabs/Objects/Resources/presets.txt"))
        {
            StreamWriter writer = new StreamWriter("Assets/Prefabs/Objects/Resources/presets.txt", false);
            foreach (var item in prefabAmount)
            {
                writer.WriteLine(item);
            }

            writer.Close();
        }
    }

}
