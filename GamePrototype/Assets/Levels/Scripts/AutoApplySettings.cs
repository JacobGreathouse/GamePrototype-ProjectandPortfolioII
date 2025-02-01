using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A bit of a quick and dirty hack of a class to automatically apply player prefs to 
/// player settings on scene load. 
/// </summary>

public class AutoApplySettings : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] gamemanager _manager;

    // Start is called before the first frame update
    void Start()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        int sensLevel = PlayerPrefs.GetInt("Sens", 300);

        _audioMixer.SetFloat("Master", Mathf.Log10(masterVol) * 20);
        _audioMixer.SetFloat("Music", Mathf.Log10(musicVol) * 20);
        _audioMixer.SetFloat("SFX", Mathf.Log10(sfxVol) * 20);

        if(_manager != null)
        {
            // normalize sensitivity as an int between 100 and 600
            //int convertSense = (int)((sensLevel * 500) + 100);

            //_manager.sensitivityChanged(convertSense);
            _manager.sensitivityChanged(sensLevel);
        }

    }

}
