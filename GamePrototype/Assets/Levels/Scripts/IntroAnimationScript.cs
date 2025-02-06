using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimationScript : MonoBehaviour
{
    [SerializeField] Animator _mainCameraAnimator;
    [SerializeField] Animator _portalAnimator;
    [SerializeField] GameObject _MainMenu;

    public void SpawnPortal()
    {
        _portalAnimator.SetTrigger("Spawn");
    }

    public void Play()
    {
        _mainCameraAnimator.SetTrigger("PlayIntro");
    }
}
