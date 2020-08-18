using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip soundShuffle;
    AudioSource myAudio;

    public static SoundManager instance;

    private void Awake()
    {
        if (SoundManager.instance == null)
            SoundManager.instance = this;
    }

    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        myAudio.PlayOneShot(soundShuffle);
    }

    void Update()
    {
        
    }
}
