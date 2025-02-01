using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] AudioSource audioFeedback;
    [SerializeField] AudioClip[] audSFXFeedback;
    [SerializeField][Range(0, 1)] float audVol;

    [Header("---- Saved Settings -----")]
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider SensSlider;

    bool playsound = false;
    private void Start()
    {


        masterVolSet(PlayerPrefs.GetFloat("MasterVolume", 0.8f));
        musicVolSet(PlayerPrefs.GetFloat("MusicVolume", 0.75f));
        SFXVolSet(PlayerPrefs.GetFloat("SFXVolume", 0.75f));
        SensitivitySlider(PlayerPrefs.GetInt("Sens", 300));
        
        MasterSlider.Equals(PlayerPrefs.GetFloat("MasterVolume", 0.8f));
        MusicSlider.Equals(PlayerPrefs.GetFloat("MusicVolume", 0.75f));
        SFXSlider.Equals(PlayerPrefs.GetFloat("SFXVolume", 0.75f));

        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        //if (SensSlider != null)
        SensSlider.value = PlayerPrefs.GetInt("Sens", 300);

        playsound = true;
    }

    public void masterVolSet(float vol)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(vol) * 20);

        PlayerPrefs.SetFloat("MasterVolume", vol);
        PlayerPrefs.Save();
    }

    public void musicVolSet(float vol)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(vol) * 20);

        PlayerPrefs.SetFloat("MusicVolume", vol);
        PlayerPrefs.Save();
    }

    public void SFXVolSet(float vol)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(vol) * 20);

        if(!audioFeedback.isPlaying && playsound == true)
            audioFeedback.PlayOneShot(audSFXFeedback[Random.Range(0, audSFXFeedback.Length)], audVol);

        PlayerPrefs.SetFloat("SFXVolume", vol);
        PlayerPrefs.Save();
    }

    public void SensitivitySlider(float val)
    {

        //int convertSense = (int)((val * 500) + 100);
        //Debug.Log(convertSense.ToString() + " ||| " + val.ToString());
        PlayerPrefs.SetInt("Sens", (int)val);
        PlayerPrefs.Save();

        if (gamemanager.instance != null)
        {
            // normalize sensitivity as an int between 100 and 600


            gamemanager.instance.sensitivityChanged((int)val);

            //gamemanager.instance.sensitivityChanged((int)val);

        }
    }

}
