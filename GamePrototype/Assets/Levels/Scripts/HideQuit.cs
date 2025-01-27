using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideQuit : MonoBehaviour
{
    [SerializeField] GameObject _quitButton;

    private void Start()
    {

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _quitButton.SetActive(false);
        }
    }
}
