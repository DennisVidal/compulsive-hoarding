using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightPoint : MonoBehaviour
{
    [Tooltip("Should the object slowly or abruptly scale back to zero?")]
    public bool blink = true;
    [Tooltip("Maximum scale the object can have when scaling.")]
    public float maxScale = 1;
    [Tooltip("Amount of time it takes for the object to reach its max scale.")]
    public float timeToScale = 1;

    [Tooltip("Billboard for the spectator.")]
    public Billboard billboardSpectator;
    [Tooltip("Billboard for the player.")]
    public Billboard billdboardPlayer;

    [Tooltip("Layer that only the spectator can see => the layer that the billboard for the spectator has to be on.")]
    public int onlySpectatorLayerIndex;
    [Tooltip("Layer that only the player can see => the layer that the billboard for the player has to be on.")]
    public int onlyPlayerLayerIndex;

    protected float timePassed = 0.0f;
    protected bool scaleUp = true;

    GameObject spectatorObject;
    GameObject playerObject;

    void Start()
    {
        if(billboardSpectator)
        {
            spectatorObject = billboardSpectator.gameObject;
            spectatorObject.layer = onlySpectatorLayerIndex;

            for (int i = 0; i < spectatorObject.transform.childCount; i++)
            {
                spectatorObject.transform.GetChild(i).gameObject.layer = onlySpectatorLayerIndex;
            }
        }

        if (billdboardPlayer)
        {
            playerObject = billdboardPlayer.gameObject;
            playerObject.layer = onlyPlayerLayerIndex;

            for (int i = 0; i < playerObject.transform.childCount; i++)
            {
                playerObject.transform.GetChild(i).gameObject.layer = onlyPlayerLayerIndex;
            }
        }

        if (spectatorObject)
        {
            spectatorObject.transform.localScale = Vector3.zero;
        }

        if(playerObject)
        {
            playerObject.transform.localScale = Vector3.zero;
        }    
    }

    protected void Update()
    {
        timePassed += Time.deltaTime;

        if(timePassed > timeToScale)
        {
            timePassed = 0.0f;
            scaleUp = !scaleUp;
        }

        float percentage = timePassed / timeToScale;

        if(!blink)
        {
            if(!scaleUp)
            {
                percentage = (timeToScale - timePassed) / timeToScale;
            }
        }

        float newScale = maxScale * percentage;

        if(newScale < 0.0f)
        {
            newScale = 0.0f;
        }

        if (newScale > maxScale)
        {
            newScale = maxScale;
        }

        if (spectatorObject)
        {
            spectatorObject.transform.localScale = new Vector3(newScale, newScale, 1.0f);
        }

        if (playerObject)
        {
            playerObject.transform.localScale = new Vector3(newScale, newScale, 1.0f);
        }
    }

    public void SetCameras(Camera spectatorCam, Camera playerCam)
    {
        billboardSpectator.cameraToLookAt = spectatorCam;
        billdboardPlayer.cameraToLookAt = playerCam;

        if (billboardSpectator)
        {
            spectatorObject = billboardSpectator.gameObject;

            if (spectatorObject)
            {
                spectatorObject.transform.localScale = Vector3.zero;
            }
        }

        if (billdboardPlayer)
        {
            playerObject = billdboardPlayer.gameObject;

            if (playerObject)
            {
                playerObject.transform.localScale = Vector3.zero;
            }
        }
    } 
}
