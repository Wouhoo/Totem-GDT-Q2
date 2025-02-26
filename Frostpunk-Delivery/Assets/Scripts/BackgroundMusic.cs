using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    // Script for managing the background music player.
    private AudioSource audioSource;
    [SerializeField] private bool isLevelTheme = false;
    [SerializeField] private float loopPoint = 15f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = MainManager.instance.musicVolume;

        if (isLevelTheme)
        {
            StartCoroutine(LoopLevelTheme());
        }
        else
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private IEnumerator LoopLevelTheme()
    {
        while (true)
        {
            audioSource.Play();
            double startTime = AudioSettings.dspTime;
            double endTime = startTime + audioSource.clip.length - loopPoint;

            while (AudioSettings.dspTime < endTime)
            {
                yield return null;
            }

            audioSource.time = (float)loopPoint;
        }
    }
}
