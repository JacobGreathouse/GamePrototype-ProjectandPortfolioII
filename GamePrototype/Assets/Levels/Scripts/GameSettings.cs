using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] AudioSource audioFeedback;
    [SerializeField] AudioClip[] audSFXFeedback;
    [SerializeField][Range(0, 1)] float audVol;

    public void masterVolSet(float vol)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(vol) * 20);
    }

    public void musicVolSet(float vol)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(vol) * 20);
    }

    public void SFXVolSet(float vol)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(vol) * 20);

        if(!audioFeedback.isPlaying)
        audioFeedback.PlayOneShot(audSFXFeedback[Random.Range(0, audSFXFeedback.Length)], audVol);
    }


}
