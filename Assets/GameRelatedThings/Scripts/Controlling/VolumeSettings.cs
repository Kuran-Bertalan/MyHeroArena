using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] Slider volume_slider;
    AudioSource source;
    float musicVolume = 1f;


    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        source.volume = musicVolume;
    }

    public void setting(float volume)
    {
        musicVolume = volume;
    }

    public void defaultSetting()
    {
        musicVolume = 0.5f;
        volume_slider.value = 0.5f;
    }
}
