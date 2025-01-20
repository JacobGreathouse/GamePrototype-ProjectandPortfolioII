using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    // SERIALIZED --------------------------------------------
    [SerializeField] GameObject _loadingText;
    [SerializeField] GameObject _continueText;


    public GameObject LoadingText => _loadingText;
    public GameObject ContinueText => _continueText;
}
