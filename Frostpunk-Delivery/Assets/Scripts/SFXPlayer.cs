using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    // Script for managing the sound effect player, which is (currently) the central game object that plays all sound effects
    private AudioSource audioSource;
    [SerializeField] AudioClip buttonClickSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = MainManager.instance.sfxVolume;
    }

    public void ClickButtonSound()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}
