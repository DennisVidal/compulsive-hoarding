using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMP_Wegthrowable : Valve.VR.InteractionSystem.Throwable
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.RegisterTrash(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        GarbageCanObjectCounter ccc;

        if (other.TryGetComponent<GarbageCanObjectCounter>(out ccc))
        {
            //Debug.Log("Sucess");
            GameEvents.Instance.UnregisterTrash(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GarbageCanObjectCounter ccc;

        if (other.TryGetComponent<GarbageCanObjectCounter>(out ccc))
        {
            //Debug.Log("Sucess");
            GameEvents.Instance.RegisterTrash(this.gameObject);
        }
    }


}
