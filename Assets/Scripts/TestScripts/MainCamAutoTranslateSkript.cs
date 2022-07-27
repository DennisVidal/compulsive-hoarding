using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamAutoTranslateSkript : MonoBehaviour
{
    bool moveRight = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveRight)
            transform.Translate(5.0f * Time.deltaTime, 0, 0);
        else
            transform.Translate(-5.0f * Time.deltaTime, 0, 0);

        if (transform.position.x >= 20)
            moveRight = false;
        if (transform.position.x <= -20)
            moveRight = true;
    }
}
