using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Highlights a point in the scene by double-clicking with the left mouse button.</summary>
public class SpectatorHighlighting : MonoBehaviour
{
    [Tooltip("Material that will be used for the highlighted objects, if null the default SteamVR highlight material will be used.")]
    public Material mat;

    [Tooltip("How long should a point in space be highlighted for? (Seconds)")]
    public float highlightPointDuration = 2.0f;
    [Tooltip("How long should an interactable object be highlighted for? (Seconds)")]
    public float highlightObjectDuration = 5.0f;

    //Prefab that is spawned on the hit point
    [Tooltip("Prefab that is spawned when highlighting a point in space.")]
    public GameObject highlightPrefab;

    //Raycast length and layers used for the raycast
    [Tooltip("Length of the raycast from the spectator camera into the scene.")]
    public float maxRaycastLength = 50.0f;
    [Tooltip("Layer Mask that is used for the raycast into the scene.")]
    public LayerMask layerMask;

    //Cameras, get handed to the Highlight-Prefab for billboard effect
    [Tooltip("Camera of the spectator that gets handed to the highlight prefab.")]
    public Camera spectatorCamera;
    [Tooltip("Camera of the player that gets handed to the highlight prefab.")]
    public Camera playerCamera;

    [Tooltip("Index of the layer that only the spectator camera can see.")]
    public int spectatorOnlyLayerIndex;
    [Tooltip("Index of the layer that only the player camera can see.")]
    public int playerOnlyLayerIndex;

    [Tooltip("How long till it does not count as a double-click anymore?")]
    [Range(0.01f, 1)]
    public float doubleClickDuration = 0.3f;
    
    protected float lastClickTime;
    protected GameObject currentHighlightedPoint;
    protected HighlightInteractableObject currentHighlightedInteractableObjectScript;

    protected float currentLifetime;
    protected float currentMaxLifetime; 

    protected void Awake()
    {
        spectatorCamera.cullingMask = spectatorCamera.cullingMask & ~(1 << playerOnlyLayerIndex);
        playerCamera.cullingMask = playerCamera.cullingMask & ~(1 << spectatorOnlyLayerIndex);
    }

    protected void Update()
    {
        currentLifetime += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if(timeSinceLastClick <= doubleClickDuration)
            {
                HighlightOnRaycastHit();
            }

            lastClickTime = Time.time;    
        }

        if (currentLifetime > currentMaxLifetime)
        {
            RemoveHighlight();
        }
    }

    /// <summary>Highlights a point or an object on the mouse position.</summary>
    public void HighlightOnRaycastHit()
    {
        //Raycast into the scene based on the mouse location on the screen
        RaycastHit hit;
        Ray ray = spectatorCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, maxRaycastLength, layerMask))
        {        
            //Remove last highlight
            RemoveHighlight();

            currentHighlightedInteractableObjectScript = hit.collider.gameObject.GetComponent<HighlightInteractableObject>();
            if (currentHighlightedInteractableObjectScript)
            {
                HighlightInteractableObject();
            }
            else
            {
                HighlightPoint(hit.point);
            }
        }
    }

    /// <summary>Highlights a point in the scene.</summary>
    /// <param name="position">Position of the highlighted point.</param>
    protected void HighlightPoint(Vector3 position)
    {
        if (highlightPrefab)
        {
            currentHighlightedPoint = Instantiate(highlightPrefab, position, Quaternion.identity);
            HighlightPoint highlightPointScript = currentHighlightedPoint.GetComponent<HighlightPoint>();
            highlightPointScript.SetCameras(spectatorCamera, playerCamera);
            highlightPointScript.onlyPlayerLayerIndex = playerOnlyLayerIndex;
            highlightPointScript.onlySpectatorLayerIndex = spectatorOnlyLayerIndex;

            currentMaxLifetime = highlightPointDuration;
            currentLifetime = 0.0f;
        }
    }

    /// <summary>Highlights an interactable object.</summary>
    void HighlightInteractableObject()
    {
        currentHighlightedInteractableObjectScript.HighlightOff();
        currentHighlightedInteractableObjectScript.HighlightOn(mat);

        currentMaxLifetime = highlightObjectDuration;
        currentLifetime = 0.0f;
    }

    /// <summary>Removes an existing highlight.</summary>
    public void RemoveHighlight()
    {
        if(currentHighlightedPoint)
        {
            Destroy(currentHighlightedPoint);
        }

        if(currentHighlightedInteractableObjectScript)
        {
            currentHighlightedInteractableObjectScript.HighlightOff();
            currentHighlightedInteractableObjectScript = null;
        }
    }  
}
