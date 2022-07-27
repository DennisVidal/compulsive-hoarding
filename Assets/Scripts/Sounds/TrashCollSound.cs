using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCollSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public float triggerMagnitude = 1.0f;
    float targetMuteDuration = 5.0f;
    float passedMuteDuration = 0.0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = true;
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > triggerMagnitude)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    void Update()
    {
        passedMuteDuration += Time.deltaTime;
        if(passedMuteDuration >= targetMuteDuration)
        {
            audioSource.mute = false;
            enabled = false; //disable Update, collision should be further called.
        }
    }
}
