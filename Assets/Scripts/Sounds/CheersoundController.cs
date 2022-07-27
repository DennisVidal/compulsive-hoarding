using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheersoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCheerSound()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
