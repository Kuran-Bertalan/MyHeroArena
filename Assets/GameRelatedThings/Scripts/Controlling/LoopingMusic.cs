using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingMusic : MonoBehaviour
{
    public AudioSource Source_music;
    public AudioClip Starter_music;

    // Start is called before the first frame update
    void Start()
    {
        Source_music.PlayOneShot(Starter_music);
        Source_music.PlayScheduled(AudioSettings.dspTime + Starter_music.length);
    }
}
