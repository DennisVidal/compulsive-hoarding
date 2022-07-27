using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Highlights objects like the highlighting of SteamVR interactables 
//Based on the the 'CreateHighlightRenderers()' and 'UpdateHighlightRenderers()' functions in the Interactable object scripts of SteamVR
public class HighlightInteractableObject : MonoBehaviour
{
    [Tooltip("Material that is used for highlighting.")]
    [SerializeField] protected Material highlightMaterial;
    protected Material defaultHighlightMaterial;

    protected MeshRenderer[] highlightMeshRenderers;
    protected MeshRenderer[] existingMeshRenderers;

    protected SkinnedMeshRenderer[] highlightSkinnedMeshRenderers;
    protected SkinnedMeshRenderer[] existingSkinnedMeshRenderers;
    protected GameObject highlightHolderObject;

    [Tooltip("Should the object itself not be highlighted?")]
    [SerializeField] protected bool ignoreSelf = false;
    [Tooltip("Which objects should be highlighted?")]
    [SerializeField] protected List<GameObject> objectsToHighlight = new List<GameObject>();

    void Start()
    {
        
        if (!objectsToHighlight.Contains(this.gameObject) && !ignoreSelf)
        {
            objectsToHighlight.Add(this.gameObject);
        }
        

        if (highlightMaterial == null)
        {
            defaultHighlightMaterial = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));
            highlightMaterial = defaultHighlightMaterial;

            if (highlightMaterial == null)
            {
                Debug.LogError("Material 'SteamVR_HoverHighlight' not found!");
            }
        }
    }

    protected void Update()
    {
        UpdateHighlighting();
    }

    public void HighlightOn(Material mat = null)
    {
        if(mat)
        {
            highlightMaterial = mat;
        }
        else
        {
            highlightMaterial = defaultHighlightMaterial;
            if (!highlightMaterial)
            {
                return;
            }
        }
        

        int highlightedObjectsCount = objectsToHighlight.Count;

        if(highlightedObjectsCount == 0)
        {
            return;
        }
      
        List<SkinnedMeshRenderer> existingSMRList = new List<SkinnedMeshRenderer>();

        for (int i = 0; i < highlightedObjectsCount; i++)
        {
            SkinnedMeshRenderer sMR = objectsToHighlight[i].GetComponent<SkinnedMeshRenderer>();

            if(sMR)
            {
                existingSMRList.Add(sMR);
            }
        }

        int existingSkinnedMeshRendererCount = existingSMRList.Count;

        existingSkinnedMeshRenderers = new SkinnedMeshRenderer[existingSkinnedMeshRendererCount];
        highlightSkinnedMeshRenderers = new SkinnedMeshRenderer[existingSkinnedMeshRendererCount];

        highlightHolderObject = new GameObject("HighlightHolderObject");
        Vector3 newScale = highlightHolderObject.transform.localScale;
        newScale *= 1.01f;
        highlightHolderObject.transform.localScale = newScale;

        for (int i = 0; i < existingSkinnedMeshRendererCount; i++)
        {
            existingSkinnedMeshRenderers[i] = objectsToHighlight[i].GetComponent<SkinnedMeshRenderer>();
            GameObject newSkinnedMeshRendererHolder = new GameObject("SkinnedMeshRendererHolder");
            newSkinnedMeshRendererHolder.transform.parent = highlightHolderObject.transform;

            SkinnedMeshRenderer newSkinnedMeshRenderer = newSkinnedMeshRendererHolder.AddComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer existingSkinnedMeshRenderer = existingSkinnedMeshRenderers[i];

            Material[] materials = new Material[existingSkinnedMeshRenderer.sharedMaterials.Length];

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j] = highlightMaterial;
            }

            newSkinnedMeshRenderer.sharedMaterials = materials;
            newSkinnedMeshRenderer.sharedMesh = existingSkinnedMeshRenderer.sharedMesh;
            newSkinnedMeshRenderer.rootBone = existingSkinnedMeshRenderer.rootBone;
            newSkinnedMeshRenderer.bones = existingSkinnedMeshRenderer.bones;
            newSkinnedMeshRenderer.updateWhenOffscreen = existingSkinnedMeshRenderer.updateWhenOffscreen;

            highlightSkinnedMeshRenderers[i] = newSkinnedMeshRenderer;
        }

        List<MeshFilter> existingMFList = new List<MeshFilter>();

        for (int i = 0; i < highlightedObjectsCount; i++)
        {
            MeshFilter mF = objectsToHighlight[i].GetComponent<MeshFilter>();

            if (mF)
            {
                existingMFList.Add(mF);
            }
        }

        int existingMeshFilterCount = existingMFList.Count;

        MeshFilter[] existingMeshFilters = new MeshFilter[existingMeshFilterCount]; //this.GetComponentsInChildren<MeshFilter>(true);
        existingMeshRenderers = new MeshRenderer[existingMeshFilterCount];
        highlightMeshRenderers = new MeshRenderer[existingMeshFilterCount];

        for (int i = 0; i < existingMeshFilterCount; i++)
        {
            existingMeshFilters[i] = objectsToHighlight[i].GetComponent<MeshFilter>();
            MeshFilter existingMeshFilter = existingMeshFilters[i];
            MeshRenderer existingMeshRenderer = existingMeshFilter.GetComponent<MeshRenderer>();

            if (existingMeshFilter == null || existingMeshRenderer == null)
            {
                continue;
            }               

            GameObject newMeshFilterHolder = new GameObject("MeshFilterHolder");
            newMeshFilterHolder.transform.parent = highlightHolderObject.transform;
            MeshRenderer newMeshRenderer = newMeshFilterHolder.AddComponent<MeshRenderer>();
            MeshFilter newMeshFilter = newMeshFilterHolder.AddComponent<MeshFilter>();
            newMeshFilter.sharedMesh = existingMeshFilter.sharedMesh;          

            Material[] materials = new Material[existingMeshRenderer.sharedMaterials.Length];

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j] = highlightMaterial;
            }

            newMeshRenderer.sharedMaterials = materials;

            highlightMeshRenderers[i] = newMeshRenderer;
            existingMeshRenderers[i] = existingMeshRenderer;
        }

        UpdateHighlighting();
    }

    public void HighlightOff()
    {
        if(highlightHolderObject)
        {
            Destroy(highlightHolderObject);
        }
    }
    void UpdateHighlighting()
    {
        if (!highlightHolderObject)
        {
            return;
        }
            
        for (int i = 0; i < existingSkinnedMeshRenderers.Length; i++)
        {
            SkinnedMeshRenderer existingSkinnedMeshRenderer = existingSkinnedMeshRenderers[i];
            SkinnedMeshRenderer highlightSkinnedMeshRenderer = highlightSkinnedMeshRenderers[i];

            if (existingSkinnedMeshRenderer && highlightSkinnedMeshRenderer) //&& attachedToHand == false
            {
                highlightSkinnedMeshRenderer.transform.position = existingSkinnedMeshRenderer.transform.position;
                highlightSkinnedMeshRenderer.transform.rotation = existingSkinnedMeshRenderer.transform.rotation;
                highlightSkinnedMeshRenderer.transform.localScale = existingSkinnedMeshRenderer.transform.lossyScale;
                highlightSkinnedMeshRenderer.localBounds = existingSkinnedMeshRenderer.localBounds;
                highlightSkinnedMeshRenderer.enabled =  existingSkinnedMeshRenderer.enabled && existingSkinnedMeshRenderer.gameObject.activeInHierarchy; //&& isHovering

                int blendShapeCount = existingSkinnedMeshRenderer.sharedMesh.blendShapeCount;
                for (int j = 0; j < blendShapeCount; j++)
                {
                    highlightSkinnedMeshRenderer.SetBlendShapeWeight(j, existingSkinnedMeshRenderer.GetBlendShapeWeight(j));
                }
            }
            else if (highlightSkinnedMeshRenderer)
            {
                highlightSkinnedMeshRenderer.enabled = false;
            }              
        }

        for (int i = 0; i < highlightMeshRenderers.Length; i++)
        {
            MeshRenderer existingMeshRenderer = existingMeshRenderers[i];
            MeshRenderer highlightMeshRenderer = highlightMeshRenderers[i];

            if (existingMeshRenderer && highlightMeshRenderer) // && attachedToHand == false
            {
                highlightMeshRenderer.transform.position = existingMeshRenderer.transform.position;
                highlightMeshRenderer.transform.rotation = existingMeshRenderer.transform.rotation;
                highlightMeshRenderer.transform.localScale = existingMeshRenderer.transform.lossyScale;
                highlightMeshRenderer.enabled = existingMeshRenderer.enabled && existingMeshRenderer.gameObject.activeInHierarchy; //&& isHovering
            }
            else if (highlightMeshRenderer)
            {
                highlightMeshRenderer.enabled = false;
            }              
        }
    }

    protected void OnDestroy()
    {
        HighlightOff();
    }
}
