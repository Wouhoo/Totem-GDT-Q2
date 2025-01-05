using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    // Script for managing the music slider in the main menu & in-game pause screen
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        slider.value = MainManager.instance.musicVolume;  // Initialize the slider with the volume value stored in the main manager
        slider.onValueChanged.AddListener(UpdateVolume);  // Run UpdateVolume whenever the slider's value changes
    }

    void UpdateVolume(float volume)
    {
        audioSource.volume = volume;                // Change the assigned audio source's volume
        MainManager.instance.musicVolume = volume;  // Update the value in the main manager as well
    }
}
