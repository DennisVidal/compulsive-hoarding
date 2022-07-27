using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DrawerLinearDrive : LinearDrive
{
	public bool isInteractable;

    override protected void Start()
    {
		base.Start();
		isInteractable = true;
	}

    override protected void HandAttachedUpdate(Hand hand)
	{
		UpdateDrawerLinearMapping(hand.transform);

		if (hand.IsGrabEnding(this.gameObject))
		{
			hand.DetachObject(gameObject);
		}
	}

	protected void UpdateDrawerLinearMapping(Transform updateTransform)
	{	
		prevMapping = linearMapping.value;
		linearMapping.value = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(updateTransform));

		if(!isInteractable)
        {
			if(linearMapping.value < prevMapping)
            {
				linearMapping.value = prevMapping;
				return;
			}
		}

		mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = (1.0f / Time.deltaTime) * (linearMapping.value - prevMapping);
		sampleCount++;

		if (repositionGameObject)
		{
			transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
		}
	}
}
