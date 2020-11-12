using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (SoundManager.instance == null)
            SoundManager.instance = this;
    }

    public void PlaySound(string soundName)
    {
        GameObject.Find(soundName).GetComponent<AudioSource>().Play();
    }


}
