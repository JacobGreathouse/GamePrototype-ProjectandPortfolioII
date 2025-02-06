using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    // SERIALIZED --------------------------------------------
    [SerializeField] GameObject _loadingText;
    [SerializeField] GameObject _continueText;
    [SerializeField] GameObject _level1Pan;
    [SerializeField] GameObject _Level2Pan;
    [SerializeField] GameObject _Level3Pan;


    public GameObject LoadingText => _loadingText;
    public GameObject ContinueText => _continueText;

    public GameObject Level1Pan => _level1Pan;

    public GameObject Level2Pan => _Level2Pan;

    public GameObject Level3Pan => _Level3Pan;

}
