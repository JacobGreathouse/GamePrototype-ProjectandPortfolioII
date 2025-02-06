using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAnimationScript : MonoBehaviour
{
    [SerializeField] Animator _mainCameraAnimator;
    [SerializeField] Animator _portalAnimator;
    [SerializeField] Animator _secondPortalAnimator;
    [SerializeField] Animator _mainMenuAnimator;
    [SerializeField] GameObject _exitCam;
    [SerializeField] GameObject _mainMenuRef;
    bool _canSkip = false;

    private void Start()
    {
        if(_mainMenuAnimator != null) _mainMenuAnimator.enabled = false;
    }

    public void SpawnPortal()
    {
        _portalAnimator.SetTrigger("Spawn");
    }

    public void SwapExitCam()
    {
        _exitCam.transform.position = new Vector3(-71.10031f, -32.6325f, -78.34066f);
        _exitCam.transform.localRotation = Quaternion.Euler(0f, -91.543f, 0f);
    }

    public void Play()
    {
        _mainMenuRef.SetActive(false);
        _canSkip = true;
        _mainCameraAnimator.SetTrigger("PlayIntro");
    }

    public void PlayIntroText()
    {
        _mainMenuAnimator.enabled=true;
        _mainMenuAnimator.SetTrigger("PlayIntro");
    }

    public void ScalePortal()
    {
        _portalAnimator.SetTrigger("Scale");
    }

    public void LoadMap()
    {
        SceneManager.LoadScene(1);
    }

    public void Update()
    {
        if (_canSkip)
        {
            if (Input.GetButtonDown("Submit"))
            {
                LoadMap();
            }
        }
    }
}
